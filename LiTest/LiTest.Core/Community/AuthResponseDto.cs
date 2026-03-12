using System;
using System.Collections.Generic;
using System.Text;

namespace LiTest.Shared.Core.Community
{
    public record AuthResponseDto(string AccessToken, string RefreshToken);
}
