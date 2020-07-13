using App.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XPaint.Components
{
    public class UI
    {
        /// <summary>绑定下拉框</summary>
        public static void BindEnum(ToolStripComboBox cmb, Type type)
        {
            cmb.Items.Clear();
            var items = type.GetEnumInfos();
            foreach (var item in items)
                cmb.Items.Add(item);

            cmb.SelectedIndex = 0;
        }

        public static void SetEnumValue(ToolStripComboBox cmb, object enumValue)
        {
            var info = enumValue.GetEnumInfo();
            cmb.SelectedItem = info;
        }

        public static object GetEnumValue(ToolStripComboBox cmb)
        {
            var info = cmb.SelectedItem as EnumInfo;
            return info.Value;
        }

    }
}
