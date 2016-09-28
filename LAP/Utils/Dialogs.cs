using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAP.Utils
{
    public class Dialogs
    {
        public ClearUC.Dialogs.Dialog.ClickedButton ShowSamePlaylisyExistDialog()
        {
            return ClearUC.Dialogs.Dialog.ShowMessageBox(ClearUC.Dialogs.Dialog.Buttons.YesNo,
                Utils.Config.Language.Strings.SameNamePlaylistExist[0], Utils.Config.Language.Strings.SameNamePlaylistExist[1]);
        }
    }
}