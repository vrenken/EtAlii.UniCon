namespace EtAlii.UniCon.Editor
{
    public partial class ConsoleView
    {
        private bool _isTrackingTail;
        private void OnTailButtonClicked()
        {
            _isTrackingTail = !_isTrackingTail;
            _tailButton.style.backgroundColor = _isTrackingTail ? _tailButtonTrackingColor : _tailButtonNonTrackingColor;
            _previousScrollValue = _listViewScrollView.verticalScroller.value;
        }
    }    
}
