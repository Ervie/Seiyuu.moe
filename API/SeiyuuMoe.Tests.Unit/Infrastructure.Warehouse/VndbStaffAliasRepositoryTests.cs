using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Infrastructure.Warehouse.Repositories;
using SeiyuuMoe.Infrastructure.Warehouse.VndbEntities;
using SeiyuuMoe.Tests.Common.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Infrastructure.Warehouse
{
	public class VndbStaffAliasRepositoryTests
	{
		[Fact]
		public async Task GetAllRoles_GivenEmptyTable_ShouldReturnEmpty()
		{
			// Given
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbStaffAliasRepository(warehouseContext);

			// When
			var result = await repository.GetAllRoles(1);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllRoles_GivenMatchedStaffAliasIdWithoutRoles_ShouldReturnEmpty()
		{
			// Given
			const int aliasId = 5;
			const int staffId = 100;

			const string name = "Test name";
			const string originalName = "Test original name";
			const string language = "ja";
			const string description = "Test description";

			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbStaffAliasRepository(warehouseContext);

			var alias = new VndbStaffAlias
			{
				Id = staffId,
				Name = name,
				StaffAliasId = aliasId,
				OriginalName = originalName,
				Staff = new VndbStaff
				{
					Id = staffId,
					Description = description,
					Language = language
				}
			};
			await warehouseContext.AddAsync(alias);
			await warehouseContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRoles(staffId);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllRoles_GivenMatchedStaffAliasIdWithSingleRole_ShouldReturnSingle()
		{
			// Given
			const int aliasId = 5;
			const int staffId = 100;

			const string name = "Test name";
			const string originalName = "Test original name";
			const string language = "ja";
			const string description = "Test description";

			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbStaffAliasRepository(warehouseContext);

			var alias = new VndbStaffAlias
			{
				Id = staffId,
				StaffAliasId = aliasId,
				Name = name,
				OriginalName = originalName,
				Staff = new VndbStaff
				{
					Id = staffId,
					Description = description,
					Language = language
				},
				Roles = new List<VndbVisualNovelSeiyuu>
				{
					new VndbVisualNovelSeiyuu
					{
						CharacterId = 10,
						VisualNovelId = 11,
						Note = "Test role 1"
					}
				}
			};
			await warehouseContext.AddAsync(alias);
			await warehouseContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRoles(staffId);

			// Then
			using (new AssertionScope())
			{
				result.Should().ContainSingle();
				result.First().Note.Should().Be("Test role 1");
				result.First().StaffAliasId.Should().Be(aliasId);
				result.First().CharacterId.Should().Be(10);
				result.First().VisualNovelId.Should().Be(11);
			}
		}

		[Fact]
		public async Task GetAllRoles_GivenMatchedStaffAliasIdWithMultipleRole_ShouldReturnMultiple()
		{
			// Given
			const int aliasId = 5;
			const int staffId = 100;

			const string name = "Test name";
			const string originalName = "Test original name";
			const string language = "ja";
			const string description = "Test description";

			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbStaffAliasRepository(warehouseContext);

			var alias = new VndbStaffAlias
			{
				Id = staffId,
				StaffAliasId = aliasId,
				Name = name,
				OriginalName = originalName,
				Staff = new VndbStaff
				{
					Id = staffId,
					Description = description,
					Language = language
				},
				Roles = new List<VndbVisualNovelSeiyuu>
				{
					new VndbVisualNovelSeiyuu(),
					new VndbVisualNovelSeiyuu(),
					new VndbVisualNovelSeiyuu()
				}
			};
			await warehouseContext.AddAsync(alias);
			await warehouseContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRoles(staffId);

			// Then
			result.Should().HaveCount(3);
		}

		[Fact]
		public async Task GetAllRoles_GivenMultipleMatchedStaffAliasesIdWithWithNoRoles_ShouldReturnEmpty()
		{
			// Given
			const int alias1Id = 5;
			const int alias2Id = 6;
			const int staffId = 100;

			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbStaffAliasRepository(warehouseContext);

			var staff = new VndbStaff
			{
				Id = staffId
			};
			await warehouseContext.AddAsync(staff);
			await warehouseContext.SaveChangesAsync();
			var alias1 = new VndbStaffAlias
			{
				Id = staffId,
				StaffAliasId = alias1Id,
				Staff = staff
			};
			var alias2 = new VndbStaffAlias
			{
				Id = staffId,
				StaffAliasId = alias2Id,
				Staff = staff
			};
			await warehouseContext.AddAsync(alias1);
			await warehouseContext.AddAsync(alias2);
			await warehouseContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRoles(staffId);

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllRoles_GivenMultipleMatchedStaffAliasesIdWithWithSingleRole_ShouldReturnSummed()
		{
			// Given
			const int alias1Id = 5;
			const int alias2Id = 6;
			const int staffId = 100;

			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbStaffAliasRepository(warehouseContext);

			var staff = new VndbStaff
			{
				Id = staffId
			};
			var alias1 = new VndbStaffAlias
			{
				Id = staffId,
				StaffAliasId = alias1Id,
				Staff = staff,
				Roles = new List<VndbVisualNovelSeiyuu>
				{
					new VndbVisualNovelSeiyuu(),
				}
			};
			var alias2 = new VndbStaffAlias
			{
				Id = staffId,
				StaffAliasId = alias2Id,
				Staff = staff,
				Roles = new List<VndbVisualNovelSeiyuu>
				{
					new VndbVisualNovelSeiyuu(),
				}
			};
			await warehouseContext.AddAsync(staff);
			await warehouseContext.AddAsync(alias1);
			await warehouseContext.SaveChangesAsync();
			await warehouseContext.AddAsync(alias2);
			await warehouseContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRoles(staffId);

			// Then
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetAllRoles_GivenMultipleMatchedStaffAliasesIdWithWithMultipleRole_ShouldReturnSummed()
		{
			// Given
			const int alias1Id = 5;
			const int alias2Id = 6;
			const int staffId = 100;

			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbStaffAliasRepository(warehouseContext);

			var staff = new VndbStaff
			{
				Id = staffId
			};
			var alias1 = new VndbStaffAlias
			{
				Id = staffId,
				StaffAliasId = alias1Id,
				Staff = staff,
				Roles = new List<VndbVisualNovelSeiyuu>
				{
					new VndbVisualNovelSeiyuu(),
					new VndbVisualNovelSeiyuu(),
					new VndbVisualNovelSeiyuu()
				}
			};
			var alias2 = new VndbStaffAlias
			{
				Id = staffId,
				StaffAliasId = alias2Id,
				Staff = staff,

				Roles = new List<VndbVisualNovelSeiyuu>
				{
					new VndbVisualNovelSeiyuu(),
					new VndbVisualNovelSeiyuu()
				}
			};
			await warehouseContext.AddAsync(staff);
			await warehouseContext.AddAsync(alias1);
			await warehouseContext.AddAsync(alias2);
			await warehouseContext.SaveChangesAsync();

			// When
			var result = await repository.GetAllRoles(staffId);

			// Then
			result.Should().HaveCount(5);
		}

		[Fact]
		public async Task GetDistinctSeiyuuIdsAsync_GivenEmptyTable_ShouldReturnEmpty()
		{
			// Given
			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbStaffAliasRepository(warehouseContext);

			// When
			var result = await repository.GetDistinctSeiyuuIdsAsync();

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetDistinctSeiyuuIdsAsync_GivenAliasWithoutRoles_ShouldReturnEmpty()
		{
			// Given
			const int aliasId = 5;
			const int staffId = 100;

			const string language = "ja";

			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbStaffAliasRepository(warehouseContext);

			var alias = new VndbStaffAlias
			{
				Id = staffId,
				StaffAliasId = aliasId,
				Staff = new VndbStaff
				{
					Id = staffId,
					Language = language
				}
			};

			await warehouseContext.AddAsync(alias);
			await warehouseContext.SaveChangesAsync();

			// When
			var result = await repository.GetDistinctSeiyuuIdsAsync();

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetDistinctSeiyuuIdsAsync_GivenAliasWithLanguageOtherThanJapanese_ShouldReturnEmpty()
		{
			// Given
			const int aliasId = 5;
			const int staffId = 100;

			const string language = "en";

			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbStaffAliasRepository(warehouseContext);

			var alias = new VndbStaffAlias
			{
				Id = staffId,
				StaffAliasId = aliasId,
				Staff = new VndbStaff
				{
					Id = staffId,
					Language = language
				},
				Roles = new List<VndbVisualNovelSeiyuu>
				{
					new VndbVisualNovelSeiyuu(),
				}
			};

			await warehouseContext.AddAsync(alias);
			await warehouseContext.SaveChangesAsync();

			// When
			var result = await repository.GetDistinctSeiyuuIdsAsync();

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetDistinctSeiyuuIdsAsync_GivenAliasWithSingleRole_ShouldReturnId()
		{
			// Given
			const int aliasId = 5;
			const int staffId = 100;

			const string language = "ja";

			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbStaffAliasRepository(warehouseContext);

			var alias = new VndbStaffAlias
			{
				Id = staffId,
				StaffAliasId = aliasId,
				Staff = new VndbStaff
				{
					Id = staffId,
					Language = language
				},
				Roles = new List<VndbVisualNovelSeiyuu>
				{
					new VndbVisualNovelSeiyuu(),
				}
			};

			await warehouseContext.AddAsync(alias);
			await warehouseContext.SaveChangesAsync();

			// When
			var result = await repository.GetDistinctSeiyuuIdsAsync();

			// Then
			using (new AssertionScope())
			{
				result.Should().ContainSingle();
				result.First().Should().Be(staffId);
			}
		}

		[Fact]
		public async Task GetDistinctSeiyuuIdsAsync_GivenAliasWithMultipleRole_ShouldReturnId()
		{
			// Given
			const int aliasId = 5;
			const int staffId = 100;

			const string language = "ja";

			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbStaffAliasRepository(warehouseContext);

			var alias = new VndbStaffAlias
			{
				Id = staffId,
				StaffAliasId = aliasId,
				Staff = new VndbStaff
				{
					Id = staffId,
					Language = language
				},
				Roles = new List<VndbVisualNovelSeiyuu>
				{
					new VndbVisualNovelSeiyuu(),
					new VndbVisualNovelSeiyuu(),
					new VndbVisualNovelSeiyuu()
				}
			};

			await warehouseContext.AddAsync(alias);
			await warehouseContext.SaveChangesAsync();

			// When
			var result = await repository.GetDistinctSeiyuuIdsAsync();

			// Then
			using (new AssertionScope())
			{
				result.Should().ContainSingle();
				result.First().Should().Be(staffId);
			}
		}

		[Fact]
		public async Task GetDistinctSeiyuuIdsAsync_GivenMultipleAliasesWithoutRoles_ShouldReturnEmpty()
		{
			// Given
			const int aliasId1 = 5;
			const int aliasId2 = 6;
			const int staffId = 100;

			const string language = "ja";

			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbStaffAliasRepository(warehouseContext);

			var staff = new VndbStaff
			{
				Id = staffId,
				Language = language
			};
			await warehouseContext.AddAsync(staff);
			await warehouseContext.SaveChangesAsync();
			var alias1 = new VndbStaffAlias
			{
				Id = staffId,
				StaffAliasId = aliasId1,
				Staff = staff
			};
			var alias2 = new VndbStaffAlias
			{
				Id = staffId,
				StaffAliasId = aliasId2,
				Staff = staff
			};

			await warehouseContext.AddAsync(alias1);
			await warehouseContext.AddAsync(alias2);
			await warehouseContext.SaveChangesAsync();

			// When
			var result = await repository.GetDistinctSeiyuuIdsAsync();

			// Then
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetDistinctSeiyuuIdsAsync_GivenMultipleAliasesWithOnlyOneWithRoles_ShouldReturnSingle()
		{
			// Given
			const int aliasId1 = 5;
			const int aliasId2 = 6;
			const int staffId = 100;

			const string language = "ja";

			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbStaffAliasRepository(warehouseContext);

			var staff = new VndbStaff
			{
				Id = staffId,
				Language = language
			};
			await warehouseContext.AddAsync(staff);
			await warehouseContext.SaveChangesAsync();
			var alias1 = new VndbStaffAlias
			{
				Id = staffId,
				StaffAliasId = aliasId1,
				Staff = staff,
				Roles = new List<VndbVisualNovelSeiyuu>
				{
					new VndbVisualNovelSeiyuu(),
				}
			};
			var alias2 = new VndbStaffAlias
			{
				Id = staffId,
				StaffAliasId = aliasId2,
				Staff = staff
			};

			await warehouseContext.AddAsync(alias1);
			await warehouseContext.AddAsync(alias2);
			await warehouseContext.SaveChangesAsync();

			// When
			var result = await repository.GetDistinctSeiyuuIdsAsync();

			// Then
			using (new AssertionScope())
			{
				result.Should().ContainSingle();
				result.First().Should().Be(staffId);
			}
		}

		[Fact]
		public async Task GetDistinctSeiyuuIdsAsync_GivenAliasesOneOfWhichWithLanguageOtherThanJapanese_ShouldReturnJapaneseOnly()
		{
			// Given
			const int aliasId1 = 5;
			const int aliasId2 = 6;
			const int staffIdEn = 100;
			const int staffIdJa = 101;

			const string languageJa = "en";
			const string languageEn = "en";

			var warehouseContext = InMemoryDbProvider.GetWarehouseDbContext();
			var repository = new VndbStaffAliasRepository(warehouseContext);

			var staffEn = new VndbStaff
			{
				Id = staffIdEn,
				Language = languageEn
			};
			var staffJa = new VndbStaff
			{
				Id = staffIdJa,
				Language = languageJa
			};
			await warehouseContext.AddAsync(staffEn);
			await warehouseContext.AddAsync(staffJa);
			await warehouseContext.SaveChangesAsync();
			var alias1 = new VndbStaffAlias
			{
				Id = staffIdEn,
				StaffAliasId = aliasId1,
				Staff = staffEn,
				Roles = new List<VndbVisualNovelSeiyuu>
				{
					new VndbVisualNovelSeiyuu(),
				}
			};
			var alias2 = new VndbStaffAlias
			{
				Id = staffIdJa,
				StaffAliasId = aliasId2,
				Staff = staffJa,
				Roles = new List<VndbVisualNovelSeiyuu>
				{
					new VndbVisualNovelSeiyuu(),
				}
			};

			await warehouseContext.AddAsync(alias1);
			await warehouseContext.AddAsync(alias2);
			await warehouseContext.SaveChangesAsync();

			// When
			var result = await repository.GetDistinctSeiyuuIdsAsync();

			// Then
			result.Should().BeEmpty();
		}
	}
}