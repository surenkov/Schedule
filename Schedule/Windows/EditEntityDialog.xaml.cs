using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Schedule.Utils.Attributes;
using Schedule.Models;
using Schedule.Models.DataLayer;
using Schedule.Utils.Editors;
using System.Linq;
using Schedule.Controls.Editors.Editors;
using System.Collections.Generic;
using System;

namespace Schedule.Windows
{
    public delegate void ApplyEventHandler(object sender, ApplyEventArgs args);

    public class ApplyEventArgs : RoutedEventArgs
    {
        public ApplyEventArgs(RoutedEvent routedEvent, Entity item) :
            base(routedEvent)
        {
            Item = item;
        }

        public Entity Item { get; set; }
    }

    public partial class EditEntityDialog : Window
    {
        private Dictionary<PropertyInfo, IEditorControl> editors;

        public static readonly RoutedEvent ApplyEvent =
            EventManager.RegisterRoutedEvent("Apply", RoutingStrategy.Direct, typeof(ApplyEventHandler),
                typeof(EditEntityDialog));

        public event ApplyEventHandler Apply
        {
            add { AddHandler(ApplyEvent, value); }
            remove { RemoveHandler(ApplyEvent, value); }
        }

        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register("Item", typeof(Entity), typeof(EditEntityDialog));

        private Entity Item
        {
            get { return (Entity)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public EditEntityDialog(Entity entity = null, bool copy = false)
        {
            InitializeComponent();
            CloseButton.Click += (s, a) => Close();

            if (copy)
                LoadEntity(entity);
            else
                Item = entity;

            editors = new Dictionary<PropertyInfo, IEditorControl>();
            InitEditors();
        }

        private void ApplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var editor in editors)
                    editor.Value.SetObjectValue(editor.Key, Item);
                RaiseEvent(new ApplyEventArgs(ApplyEvent, Item));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadEntity(Entity entity)
        {
            using (ScheduleDbContext ctx = new ScheduleDbContext())
            {
                if (entity != null)
                {
                    Item = ctx.Set(entity.GetType()).Find(entity.Id) as Entity;
                    foreach (var rel in Item.GetType().GetProperties().
                        Where(p => p.PropertyType.IsSubclassOf(typeof(Entity))))
                        ctx.Entry(Item).Reference(rel.Name).Load();
                }
            }
        }

        private void InitEditors()
        {
            if (Item == null) return;
            EditorsFactory factory = new EditorsFactory();
            EditorsMapper mapper = new EditorsMapper();

            var properties = Item.GetType().GetProperties();
            int i = 0;

            foreach (var property in properties)
            {
                var descr = property.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                var hide = property.GetCustomAttribute(typeof(HiddenAttribute)) as HiddenAttribute;

                if (hide != null && hide.Hidden) continue;

                EditorsGrid.RowDefinitions.Add(new RowDefinition());
                string caption = descr != null ? descr.Description : property.Name;


                Label lbl = new Label { Content = caption, Margin = new Thickness(0, 5, 0, 0) };

                var ctrl = factory.CreateEditor(property.PropertyType);
                if (ctrl == null) continue;
                ctrl.Margin = new Thickness(0, 5, 0, 0);

                var editor = ctrl as IEditorControl;
                if (editor == null) continue;
                editor.Initialize(property.PropertyType);
                editor.Value = property.GetValue(Item);
                editors.Add(property, editor);

                Grid.SetColumn(lbl, 0);
                Grid.SetColumn(ctrl, 1);
                Grid.SetRow(lbl, i);
                Grid.SetRow(ctrl, i++);

                EditorsGrid.Children.Add(lbl);
                EditorsGrid.Children.Add(ctrl);
            }
        }
    }
}
