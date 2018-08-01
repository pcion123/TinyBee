namespace TinyBee.UI
{
    using System;

    public class UIClick : UIEvent
    {
        public Action<object> onClick { get; set; }

        void OnClick()
        {
            onClick.InvokeGracefully(mParameter);
        }
    }
}
