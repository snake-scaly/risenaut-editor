using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RisenautEditor
{
    interface IFileService
    {
        GameFile OpenGame();
        bool SaveGame(GameFile game);
        bool SaveGameAs(GameFile game);

        /// <summary>
        /// Ask the user if they want to save a modified game before a destructive operation.
        /// </summary>
        /// <remarks>
        /// The user is presented with a yes-no-cancel dialog. "Yes" saves the game and proceeds.
        /// "No" proceeds without saving. "Cancel" cancels the operation.
        /// </remarks>
        /// <param name="game"></param>
        /// <returns><code>true</code> if the user wants to proceed, <code>false</code> to
        /// cancel the operation.</returns>
        bool QueryGameModified(GameFile game);
    }
}
