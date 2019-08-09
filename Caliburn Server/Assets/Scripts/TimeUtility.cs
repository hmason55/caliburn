using System;

public static class TimeUtility {
    public static int CurrentUnixTimestamp() {
        return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }
}
