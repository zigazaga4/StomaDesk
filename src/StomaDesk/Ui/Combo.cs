using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using StomaDesk.Models;

namespace StomaDesk.Ui
{
    /// <summary>Filling and reading combo boxes the same way everywhere.</summary>
    public static class Combo
    {
        /// <summary>Items are shown through ToString(). A drop-down list with no match selects its first item.</summary>
        public static void Fill(ComboBox combo, IEnumerable items, object selected)
        {
            combo.BeginUpdate();
            combo.Items.Clear();
            foreach (object item in items)
                combo.Items.Add(item);
            combo.EndUpdate();

            if (selected != null && combo.Items.Contains(selected))
                combo.SelectedItem = selected;
            else if (combo.Items.Count > 0 && combo.DropDownStyle == ComboBoxStyle.DropDownList)
                combo.SelectedIndex = 0;
        }

        public static void FillChoices<T>(ComboBox combo, IEnumerable<Choice<T>> choices, T selected) where T : struct
        {
            Fill(combo, choices, null);
            SelectValue(combo, selected);
        }

        /// <summary>One item per enum value, with its Romanian label.</summary>
        public static void FillEnum<T>(ComboBox combo, Func<T, string> label, T selected) where T : struct
        {
            FillChoices(combo, Choices.FromEnum(label), selected);
        }

        public static void SelectValue<T>(ComboBox combo, T value) where T : struct
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                var choice = combo.Items[i] as Choice<T>;
                if (choice != null && choice.Value.Equals(value))
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }

        public static T Value<T>(ComboBox combo) where T : struct
        {
            var choice = combo.SelectedItem as Choice<T>;
            return choice == null ? default(T) : choice.Value;
        }

        public static T Selected<T>(ComboBox combo) where T : class
        {
            return combo.SelectedItem as T;
        }

        public static void SelectWhere<T>(ComboBox combo, Func<T, bool> match) where T : class
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                T item = combo.Items[i] as T;
                if (item != null && match(item))
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }
    }
}
