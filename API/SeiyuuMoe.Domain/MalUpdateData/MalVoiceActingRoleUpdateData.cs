namespace SeiyuuMoe.Domain.MalUpdateData
{
	public class MalVoiceActingRoleUpdateData
	{
		public long AnimeMalId { get; }

		public long CharacterMaId { get; }

		public string RoleType { get; }

		public MalVoiceActingRoleUpdateData(long animeMalId, long characterMalId, string roleType)
		{
			AnimeMalId = animeMalId;
			CharacterMaId = characterMalId;
			RoleType = roleType;
		}
	}
}