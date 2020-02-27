using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMC;

namespace AIO.APP.Views
{
    public class MainOperationController: Controller
    {
        public override void Initial()
        {
            ExecuteNavigation("Scanner");
        }

        public void Scanner()
        {
            ExecuteNavigation();
        }

        public void Validation()
        {
            ExecuteNavigation();
        }

        public void Setting()
        {
            ExecuteNavigation();
        }
    }
}
