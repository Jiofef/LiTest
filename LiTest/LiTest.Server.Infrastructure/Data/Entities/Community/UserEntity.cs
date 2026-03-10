using LiTest.Server.Infrastructure.Data.Entities.Testing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiTest.Shared.Core.Community
{
    public class UserEntity
    {
        // Metadata
        public Guid Id { get; set; }
        public Guid AvatarUid { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHashed { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;

        public DateTimeOffset? DeletedAt = null;

        //
        public List<Guid> UserTestIds { get; set; } = new();
        public List<AttemptEntity> AttemptStatuses { get; set; } = new();
    }
}