using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using Schedule.Models.DataLayer;
using Schedule.Models.ViewModels.Slices;
using Schedule.Utils;
using Schedule.Utils.Filters;
using Schedule.Utils.Conflicts;

namespace Schedule.Controls.Slices
{
    public class SliceView : ScheduleView
    {
        public static readonly DependencyProperty HorizontalHeaderTypeProperty = 
            DependencyProperty.Register("HorizontalHeaderType", typeof(Type), typeof(SliceView), 
                new PropertyMetadata(null, HorizontalHeaderTypePropertyChangedCallback));

        public static readonly DependencyProperty VerticalHeaderTypeProperty = 
            DependencyProperty.Register("VerticalHeaderType", typeof(Type), typeof(SliceView), 
                new PropertyMetadata(null, VerticalHeaderTypePropertyChangedCallback));

        public static readonly DependencyProperty VerticalHeaderProperty = 
            DependencyProperty.Register("VerticalHeader", typeof(IEnumerable), typeof(SliceView), 
                new PropertyMetadata(null, VerticalHeaderChangedCallback));

        public static readonly DependencyProperty StartDateProperty =
            DependencyProperty.Register("StartDate", typeof(DateTime), typeof(SliceView), 
                new PropertyMetadata(DateTime.Now.Date));

        public static readonly DependencyProperty EndDateProperty =
            DependencyProperty.Register("EndDate", typeof(DateTime), typeof(SliceView), 
                new PropertyMetadata(DateTime.Now.Date.AddMonths(1)));

        static SliceView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliceView), new FrameworkPropertyMetadata(typeof(SliceView)));
        }

        private static void VerticalHeaderTypePropertyChangedCallback(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var view = d as SliceView;
            if (view != null)
                view.FillVerticalHeader();
        }

        private static void HorizontalHeaderTypePropertyChangedCallback(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var view = d as SliceView;
            if (view != null)
                view.FillHorizontalHeader();
        }

        private static void VerticalHeaderChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as SliceView;
            if (view != null) view.OnVerticalHeaderChanged();
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

        public DateTime StartDate
        {
            get { return (DateTime)GetValue(StartDateProperty); }
            set { SetValue(StartDateProperty, value); }
        }

        public DateTime EndDate
        {
            get { return (DateTime)GetValue(EndDateProperty); }
            set { SetValue(EndDateProperty, value); }
        }

        public override void UpdateView()
        {
            Update();
        }

        private void OnVerticalHeaderChanged()
        {
            Update();
        }

        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            base.OnHeaderChanged(oldHeader, newHeader);
            Update();
        }

        private void Update()
        {
            using (ScheduleDbContext ctx = new ScheduleDbContext())
            {
                if (Header == null || VerticalHeader == null) return;

                ctx.Schedule.Include("Course").Include("Teacher").Include("Group").Include("Class.Building").Load();
                var itemsList = ctx.Schedule.Local.ApplyFilters(Filters).Cast<Models.Schedule>();

                var horizontalHeaderItems = Header as IEnumerable;
                var vericalHeaderItems = VerticalHeader.Cast<object>().Select(item => new SliceRowViewModel
                {
                    Header = item,
                    Items = new List<SliceCellViewModel>()
                }).ToList();

                var conflicts = new ConflictsManager().CheckAll(itemsList);
                foreach (var rowModel in vericalHeaderItems)
                {
                    foreach (var horizontalHeaderItem in horizontalHeaderItems)
                    {
                        var cell = new SliceCellViewModel
                        {
                            HorizontalValue = horizontalHeaderItem,
                            VerticalValue = rowModel.Header,
                            ScheduleView = this
                        };
                        cell.Fill(itemsList, conflicts);
                        rowModel.Items.Add(cell);
                    }
                }
                ItemsSource = vericalHeaderItems;
            }
        }
    }
}
