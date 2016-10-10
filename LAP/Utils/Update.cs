using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAP.Utils
{
    class Update
    {
        private static ClientUpdater.Engine Engine = new ClientUpdater.Engine();

        static Update()
        {
            Engine.Updater = new Updater();
        }

        public void BeginUpdate()
        {
            Engine.Update(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
        }
    }

    class Updater : ClientUpdater.IUpdate
    {
        public string Update(Version CurrentVersion)
        {
            return null;
        }
    }
}
