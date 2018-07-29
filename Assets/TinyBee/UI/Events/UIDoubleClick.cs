namespace TinyBee.UI
{
    using System;

    public class UIDoubleClick : UIEvent
    {
        public Action<object> onDoubleClick { get; set; }

        void OnDoubleClick()
        {
            onDoubleClick.InvokeGracefully(mParameter);
        }
    }
}
