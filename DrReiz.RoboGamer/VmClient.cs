using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrReiz.RoboGamer
{
    public class VmClient:IDisposable
    {
        public VmClient()
        {
            var vm_title = "Windows81 [Running] - Oracle VM VirtualBox";

            var handle = DirectGamer.FindWindow(null, vm_title);
            if (handle == IntPtr.Zero)
                throw new Exception("Окно не найдено");


            DirectGamer.RECT rect;
            DirectGamer.GetWindowRect(handle, out rect);
            this.GameScreenRect = new System.Drawing.Rectangle(rect.Left + vm_left, rect.Top + vm_top, rect.Right - rect.Left - vm_right - vm_left, rect.Bottom - rect.Top - vm_bottom - vm_top);

        }
        public readonly System.Drawing.Rectangle GameScreenRect;

        public const int vm_left = 8;
        public const int vm_right = 8;
        public const int vm_top = 52;
        public const int vm_bottom = 29;

        public System.Drawing.Bitmap Screenshot()
        {
            return DirectGamer.GetScreenImage(GameScreenRect);
        }

        public void Dispose()
        {
        }
    }
}
