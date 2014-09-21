using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Schedule.Attributes;
using Schedule.Models;
using Schedule.Models.DataLayer;
using Schedule.Utils;

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

    public partial class EditScheduleDialog : Window
    {
        public static readonly RoutedEvent ApplyEvent;
        public static readonly DependencyProperty ItemProperty;
        static EditScheduleDialog()
        {
            ItemProperty = DependencyProperty.Register("Item", typeof(Entity), typeof(EditScheduleDialog));

            ApplyEvent = EventManager.RegisterRoutedEvent("Apply", RoutingStrategy.Direct, typeof(ApplyEventHandler),
                typeof(EditScheduleDialog));
        }

        public EditScheduleDialog(Entity entity = null, bool copy = false)
        {
            InitializeComponent();
            CloseButton.Click += (s, a) => Close();

            if (copy)
            {
                using (ScheduleDbContext ctx = new ScheduleDbContext())
                {
                    if (entity != null)
                        Item = ctx.Set(entity.GetType()).Find(entity.Id) as Entity;
                    else
                        Item = null;
                }
            }
            else
            {
                Item = entity;
            }
            InitEditors();
        }

        public event ApplyEventHandler Apply
        {
            add { AddHandler(ApplyEvent, value); }
            remove { RemoveHandler(ApplyEvent, value); }
        }

        private Entity Item
        {
            get { return (Entity)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }
        private void ApplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new ApplyEventArgs(ApplyEvent, Item));
        }

        private void InitEditors()
        {
            if (Item == null) return;
            EditorsFactory factory = new EditorsFactory();
            EditorsMapper mapper = new EditorsMapper();

            var properties = Item.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            int i = 0;

            foreach (var property in properties)
            {
                var descr = property.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                var shown = property.GetCustomAttribute(typeof(NotShownAttribute)) as NotShownAttribute;

                if (shown != null && !shown.Shown) continue;
                if (property.PropertyType != typeof(string) &&
                    property.PropertyType.GetInterface("IEnumerable") != null) continue;

                EditorsGrid.RowDefinitions.Add(new RowDefinition());
                string caption = descr != null ? descr.Description : property.Name;


                Label lbl = new Label { Content = caption, Margin = new Thickness(0, 5, 0, 0) };
                var ctrl = factory.CreateControl(property.PropertyType);
                mapper.FillData(ctrl, property.PropertyType);
                ctrl.Margin = new Thickness(0, 5, 0, 0);

                DependencyProperty dp = factory.BindingProperty(factory.ControlType(property.PropertyType));
                if (dp != null)
                {
                    Binding binding = new Binding(property.Name) { Source = Item };
                    BindingOperations.SetBinding(ctrl, dp, binding);
                }

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
