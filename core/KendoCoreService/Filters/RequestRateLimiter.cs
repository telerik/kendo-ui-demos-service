namespace KendoCoreService.Filters
{
    public class RequestRateLimiter
    {
        private readonly int _maxRequests;
        private readonly int _timeWindowInSeconds;
        private readonly Queue<DateTime> _requestTimestamps = new();

        public RequestRateLimiter(int maxRequests, int timeWindowInSeconds)
        {
            _maxRequests = maxRequests;
            _timeWindowInSeconds = timeWindowInSeconds;
        }

        public bool IsRequestAllowed()
        {
            lock (_requestTimestamps)
            {
                DateTime currentTime = DateTime.UtcNow;

                while (_requestTimestamps.Count > 0 && (currentTime - _requestTimestamps.Peek()).TotalSeconds > _timeWindowInSeconds)
                {
                    _requestTimestamps.Dequeue();
                }

                if (_requestTimestamps.Count >= _maxRequests)
                {
                    return false;
                }

                _requestTimestamps.Enqueue(currentTime);
                return true;
            }
        }
    }
}
