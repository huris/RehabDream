using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muse
{
    /// <summary>
    /// 处理T型数据的委托
    /// </summary>
    /// <param name="packet"></param>
    public delegate void HandleMessage<T>(T packet);

}
