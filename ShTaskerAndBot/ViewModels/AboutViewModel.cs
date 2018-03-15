using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShTaskerAndBot.ViewModels
{
    class AboutViewModel : ViewModelBase
    {
        public bool? DialogResult { get { return false; } }

        public string Content
        {
            get
            {
                return "ShTaskerAndBot" + Environment.NewLine +
                        "Created by SheryvL" + Environment.NewLine +
                        "Address" + Environment.NewLine +
                        "2018";
            }
        }

        public string VersionText
        {
            get
            {
                var version1 = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                // For external assemblies
                // var ver2 = typeof(Assembly1.ClassOfAssembly1).Assembly.GetName().Version;
                // var ver3 = typeof(Assembly2.ClassOfAssembly2).Assembly.GetName().Version;

                return "ShTaskerAndBot v" + version1.ToString();
            }
        }
    }
}
