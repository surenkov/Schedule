using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Schedule.Models;
using Schedule.Utils;
using Schedule.Utils.Filters;
using Schedule.Utils.ValueConverters;

namespace Schedule.Controls.Editors
{
    public delegate bool CheckPropertyValueDelegate(IComparable value, IComparable compared);
    public partial class Filter : UserControl
    {
        private Control _control;
        public static readonly DependencyProperty PropertiesProperty =
            DependencyProperty.Register("Properties", typeof(ICollection<PropertyInfo>), typeof(Filter),
                new PropertyMetadata(default(ICollection<PropertyInfo>)));

        public ICollection<PropertyInfo> Properties
        {
            get { return (ICollection<PropertyInfo>) GetValue(PropertiesProperty); }
            set { SetValue(PropertiesProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(object), typeof(Filter), new PropertyMetadata(default(object)));

        public object Value
        {
            get { return (object) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private readonly List<string> _comparers = new List<string>
        {
            "=",          
            "<>",      
            "<",           
            ">",        
            "<=",  
            ">="
        };

        private readonly Type _type;

        public Filter()
        {
            InitializeComponent();

            ConditionsBox.ItemsSource = _comparers;
            ConditionsBox.SelectedIndex = 0;

            DataContext = this;
            PropertiesBox.DataContext = this;
            PropertiesBox.SelectedIndex = 0;
        }

        public Filter(Type type)
            : this()
        {
            _type = type;
            FillPropertiesBox();
            var dpd = DependencyPropertyDescriptor.FromProperty(Selector.SelectedItemProperty, typeof(ComboBox));
            if (dpd != null) dpd.AddValueChanged(PropertiesBox, PropertiesBox_PropertyChanged);
        }

        private void PropertiesBox_PropertyChanged(object sender, EventArgs eventArgs)
        {
            var property = PropertiesBox.SelectedItem as PropertyInfo;
            var factory = new FilterEditorsFactory();
            var mapper = new EditorsMapper();

            if (property != null)
            {
                var control = factory.CreateControl(property.PropertyType);
                mapper.FillData(control, property.PropertyType);

                var box = control as ComboBox;
                if (box != null) box.IsEditable = true;

                control.Margin = new Thickness(0, 0, 5, 5);
                Grid.SetColumn(control, 2);
                FilterGrid.Children.Remove(_control);
                _control = control;
                FilterGrid.Children.Add(_control);
            }
        }

        public IEnumerable<Entity> FilterEntities(IEnumerable<Entity> entities)
        {
            return entities.Where(ApplyFilterOnEntity).ToList();
        }

        private bool ApplyFilterOnEntity(Entity entity)
        {
            var property = PropertiesBox.SelectedItem as PropertyInfo;
            var converter = new StringToComparerConverter();
            var factory = new FilterEditorsFactory();

            if (property != null)
            {
                var value = (IComparable) property.GetValue(entity);
                if (value != null)
                {
                    var comparer = converter.Convert(ConditionsBox.SelectedItem as string,
                        typeof(CheckPropertyValueDelegate), null, null) as CheckPropertyValueDelegate;
                    var compared = (IComparable) _control.GetValue(factory.ValueProperty(_control));
                    if (compared != null)
                    {
                        if (compared.GetType() != value.GetType())
                            compared = (IComparable) compared.FilterConvert(value.GetType());
                        return comparer != null && comparer(value, compared);
                    }
                }
            }
            return false;
        }

        private void FillPropertiesBox()
        {
            var properties = _type.GetProperties();

            Properties = new List<PropertyInfo>(properties.Length);
            foreach (
                var property in
                    properties.Where(
                        property =>
                            property.PropertyType == typeof(string) ||
                            property.PropertyType.GetInterface("IEnumerable") == null))
                Properties.Add(property);
        }
    }
}
