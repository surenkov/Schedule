using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Schedule.Models;
using Schedule.Models.DataLayer;
using Schedule.Windows;

namespace Schedule.Controls.Editors
{
    [TemplatePart(Name = "PART_AddButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Items", Type = typeof(ComboBox))]
    public class EntitySelector : Selector
    {
        private Button _addButton;
        private ComboBox _itemsBox;

        public static readonly DependencyProperty ItemsTypeProperty;

        static EntitySelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EntitySelector),
                new FrameworkPropertyMetadata(typeof(EntitySelector)));

            ItemsTypeProperty = DependencyProperty.Register("ItemsType", typeof(Type), typeof(EntitySelector));
        }

        public Type ItemsType
        {
            get { return (Type)GetValue(ItemsTypeProperty); }
            set { SetValue(ItemsTypeProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _addButton = GetTemplateChild("PART_AddButton") as Button;
            _itemsBox = GetTemplateChild("PART_Items") as ComboBox;

            if (_addButton != null)
                _addButton.Click += AddButton_OnClick;
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (ItemsType == null) return;

            var i = Activator.CreateInstance(ItemsType) as Entity;
            var dlg = new EditScheduleDialog(i);
            dlg.Apply += delegate(object o, ApplyEventArgs args)
            {
                var item = args.Item;
                args.Handled = true;

                using (ScheduleDbContext ctx = new ScheduleDbContext())
                {
                    var properties = item.GetType().GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(Entity)));
                    foreach (var rel in properties.Select(p => p.GetValue(item) as Entity))
                        ctx.Set(rel.GetType()).Attach(rel);
                    
                    ctx.Entry(item).State = item.Id == 0 ? EntityState.Added : EntityState.Modified;
                    ctx.SaveChanges();

                    ctx.Set(item.GetType()).Load();
                    ItemsSource = ctx.Set(item.GetType()).Local;
                }
            };
            dlg.ShowInTaskbar = true;
            dlg.Show();
        }
    }
}
