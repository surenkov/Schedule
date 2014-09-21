using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Schedule.Models.ViewModels.Slices;

namespace Schedule.Controls.Slices
{
    public class SliceView : HeaderedItemsControl
    {
        static SliceView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliceView), new FrameworkPropertyMetadata(typeof(SliceView)));
        }

        public SliceView()
        {
            Header = new List<string>
            {
                "Monday",
                "Tuesday",
                "Wednesday",               
                "Thursday",
                "Friday",
                "Saturday",                
                "Sunday"
            };

            ItemsSource = new List<SliceRowViewModel>
            {
                new SliceRowViewModel {Header = "1st Period", Items = new List<object>
                {
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                }},
                new SliceRowViewModel {Header = "2nd Period", Items = new List<object>
                {
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                }},
                new SliceRowViewModel {Header = "3rd Period", Items = new List<object>
                {
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                }},
                new SliceRowViewModel {Header = "4th Period", Items = new List<object>
                {
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                }},
                new SliceRowViewModel {Header = "Fifth Period", Items = new List<object>
                {
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                    new String('c', 5),
                }},
            };
        }
    }
}
