using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfTasks.Common
{
    public interface IViewModel
    {
        void Init();

        void Init(object parameter);

        //void Init(params object[] parameters);    // Currently not used. May use it in the future
    }
}
