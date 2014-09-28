using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Schedule.Controls.Editors;
using Schedule.Models;
using Schedule.Models.DataLayer;
using Schedule.Models.ViewModels.Slices;
using Schedule.Utils.Filters;

namespace Schedule.Controls.Slices
{
    public class SliceView : HeaderedItemsControl
    {
        public static readonly DependencyProperty HorizontalHeaderTypeProperty;
        public static readonly DependencyProperty VerticalHeaderTypeProperty;
        public static readonly DependencyProperty VerticalHeaderProperty;
        public static readonly DependencyProperty FiltersProperty;

        static SliceView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliceView), new FrameworkPropertyMetadata(typeof(SliceView)));
            VerticalHeaderProperty = DependencyProperty.Register("VerticalHeader", typeof(IEnumerable),
                typeof(SliceView), new PropertyMetadata(null, VerticalHeaderChangedCallback));

            HorizontalHeaderTypeProperty = DependencyProperty.Register("HorizontalHeaderType", typeof(Type),
                typeof(SliceView), new PropertyMetadata(null, HorizontalHeaderTypePropertyChangedCallback));
            VerticalHeaderTypeProperty = DependencyProperty.Register("VerticalHeaderType", typeof(Type),
                typeof(SliceView), new PropertyMetadata(null, VerticalHeaderTypePropertyChangedCallback));
            FiltersProperty = DependencyProperty.Register("Filters", typeof(IEnumerable<Filter>), typeof(SliceView),
                new PropertyMetadata(default(IEnumerable<Filter>)));
        }

        private static async void VerticalHeaderTypePropertyChangedCallback(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var view = d as SliceView;
            if (view != null)
            {
                Type t = e.NewValue as Type;
                using (ScheduleDbContext ctx = new ScheduleDbContext())
                {
                    await ctx.Set(t).LoadAsync();
                    view.VerticalHeader = ctx.Set(t).Local;
                }
            }
        }

        private static async void HorizontalHeaderTypePropertyChangedCallback(DependencyObject d, 
            DependencyPropertyChangedEventArgs e)
        {
            var view = d as SliceView;
            if (view != null)
            {
                Type t = e.NewValue as Type;
                using (ScheduleDbContext ctx = new ScheduleDbContext())
                {
                    await ctx.Set(t).LoadAsync();
                    view.Header = ctx.Set(t).Local;
                }
            }
        }

        private static void VerticalHeaderChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as SliceView;
            if (view != null) view.OnVerticalHeaderChanged(e.NewValue);
        }

        public IEnumerable VerticalHeader
        {
            get { return (IEnumerable) GetValue(VerticalHeaderProperty); }
            set { SetValue(VerticalHeaderProperty, value); }
        }

        public Type HorizontalHeaderType
        {
            get { return (Type) GetValue(HorizontalHeaderTypeProperty); }
            set { SetValue(HorizontalHeaderTypeProperty, value); }
        }

        public Type VerticalHeaderType
        {
            get { return (Type) GetValue(VerticalHeaderTypeProperty); }
            set { SetValue(VerticalHeaderTypeProperty, value); }
        }

        public IEnumerable<Filter> Filters
        {
            get { return (IEnumerable<Filter>) GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }

        public void UpdateView()
        {
            Update();
        }

        private void OnVerticalHeaderChanged(object newHeader)
        {
            var header = newHeader as IEnumerable<Entity>;
            if (header != null)
            {
                var list = header.Select(item => new SliceRowViewModel
                {
                    Header = item.ToString(),
                    Items = new List<SliceCellViewModel>()
                }).ToList();
                ItemsSource = list;
            }
            Update();
        }

        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            base.OnHeaderChanged(oldHeader, newHeader);
            Update();
        }

        private async void Update()
        {
            using (ScheduleDbContext ctx = new ScheduleDbContext())
            {
                await ctx.Set<Models.Schedule>().LoadAsync();
                var list = ctx.Set<Models.Schedule>().Local.ApplyFilters(Filters);

                var verticalHeaderItems = VerticalHeader as IEnumerable<Entity>;
                var horizontalHeaderItems = Header as IEnumerable<Entity>;

                if (verticalHeaderItems != null && horizontalHeaderItems != null)
                {
                    foreach (var verticalItem in verticalHeaderItems)
                    {

                    }
                }
            }
        }
    }
}
