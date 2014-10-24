using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Schedule.Models;
using Schedule.Controls.Editors.Filters;
using Schedule.Utils.Attributes;
using System.Windows.Data;
using Schedule.Models.ViewModels;

namespace Schedule.Controls.Editors
{
    public partial class Filter : UserControl
    {
        private IFilterControl control;
        private readonly Type type;

        public static readonly Dictionary<Type, Type> filterTypes = new Dictionary<Type, Type>();

        public ICollection<PropertyInfo> Properties
        {
            get { return (ICollection<PropertyInfo>)GetValue(PropertiesProperty); }
            set { SetValue(PropertiesProperty, value); }
        }
        public static readonly DependencyProperty PropertiesProperty =
            DependencyProperty.Register("Properties", typeof(ICollection<PropertyInfo>), typeof(Filter),
                new PropertyMetadata(default(ICollection<PropertyInfo>)));

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(Filter), new PropertyMetadata(null));

        public static void RegisterFilterControl(Type val, Type control)
        {
            filterTypes.Add(val, control);
        }

        private static Control CreateFilterControl(Type t)
        {
            while (t != null)
            {
                if (filterTypes.ContainsKey(t))
                    return Activator.CreateInstance(filterTypes[t]) as Control;
                t = t.BaseType;
            }
            return null;
        }

        static Filter()
        {
            RegisterFilterControl(typeof(int), typeof(IntegerFilter));
            RegisterFilterControl(typeof(string), typeof(StringFilter));
            RegisterFilterControl(typeof(Enum), typeof(EnumFilter));
            RegisterFilterControl(typeof(Entity), typeof(EntityFilter));
            RegisterFilterControl(typeof(DateTime), typeof(DateFilter));
        }

        public Filter()
        {
            InitializeComponent();
            DataContext = this;
            PropertiesBox.DataContext = this;
        }

        public Filter(Type t) : this()
        {
            type = t;
            FillPropertiesBox();
            var dpd = DependencyPropertyDescriptor.FromProperty(Selector.SelectedItemProperty, typeof(ComboBox));
            if (dpd != null) dpd.AddValueChanged(PropertiesBox, PropertiesBox_PropertyChanged);
        }

        private void PropertiesBox_PropertyChanged(object sender, EventArgs eventArgs)
        {
            if (control != null)
                BindingOperations.ClearAllBindings(control as Control);

            var property = PropertiesBox.SelectedItem as PropertyInfo;
            if (property != null)
            {
                var control = CreateFilterControl(property.PropertyType);
                control.Margin = new Thickness(0, 0, 5, 5);
                Grid.SetColumn(control, 2);

                FilterGrid.Children.Remove(this.control as Control);
                FilterGrid.Children.Add(control);

                this.control = control as IFilterControl;
                this.control.SetSourceType(property.PropertyType);

                ConditionsBox.ItemsSource = this.control.Comparers();
                ConditionsBox.SelectedIndex = 0;

                DependencyProperty dp = this.control.ValueProperty();
                if (dp != null)
                {
                    Binding binding = new Binding("Value") { Source = this, Mode = BindingMode.TwoWay };
                    BindingOperations.SetBinding(control, dp, binding);
                }
            }
        }

        public IEnumerable<Entity> FilterEntities(IEnumerable<Entity> entities)
        {
            return entities.Where(ApplyFilterOnEntity);
        }

        private bool ApplyFilterOnEntity(Entity entity)
        {
            var property = PropertiesBox.SelectedItem as PropertyInfo;
            var comparer = ConditionsBox.SelectedValue as CheckPropertyValueDelegate;

            if (property != null && comparer != null && control.Value() != null)
                return comparer(control.Value(), property.GetValue(entity));
            return false;
        }

        private void FillPropertiesBox()
        {
            var properties = type.GetProperties();

            Properties = new List<PropertyInfo>(properties.Length);
            foreach (
                var property in
                    properties.Where(
                        property =>
                            property.GetCustomAttribute<HiddenAttribute>() == null ||
                            property.GetCustomAttribute<HiddenAttribute>() != null &&
                            !property.GetCustomAttribute<HiddenAttribute>().Hidden))
                Properties.Add(property);
        }
    }
}
