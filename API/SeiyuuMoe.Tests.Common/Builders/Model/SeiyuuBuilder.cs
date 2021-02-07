﻿using SeiyuuMoe.Domain.Entities;
using System;

namespace SeiyuuMoe.Tests.Common.Builders.Model
{
	public class SeiyuuBuilder
	{
		private string _name = string.Empty;
		private string _imageUrl = string.Empty;
		private DateTime? _birthday = null;
		private string _japaneseName = string.Empty;
		private string _about = string.Empty;
		private long _malId;
		private long _vndbId;
		private int _popularity;
		private Guid _id;
		private DateTime _modificationDate;

		public Seiyuu Build() => new Seiyuu
		{
			Id = _id,
			Name = _name,
			ImageUrl = _imageUrl,
			MalId = _malId,
			VndbId = _vndbId,
			Birthday = _birthday,
			Popularity = _popularity,
			KanjiName = _japaneseName,
			About = _about,
			ModificationDate = _modificationDate
		};


		public SeiyuuBuilder WithId(Guid id)
		{
			_id = id;
			return this;
		}

		public SeiyuuBuilder WithName(string name)
		{
			_name = name;
			return this;
		}

		public SeiyuuBuilder WithImageUrl(string imageUrl)
		{
			_imageUrl = imageUrl;
			return this;
		}

		public SeiyuuBuilder WithMalId(long malId)
		{
			_malId = malId;
			return this;
		}

		public SeiyuuBuilder WithVndbId(long vndbId)
		{
			_vndbId = vndbId;
			return this;
		}

		public SeiyuuBuilder WithBirthday(DateTime? birthday)
		{
			_birthday = birthday;
			return this;
		}

		public SeiyuuBuilder WithPopularity(int popularity)
		{
			_popularity = popularity;
			return this;
		}

		public SeiyuuBuilder WithJapaneseName(string japaneseName)
		{
			_japaneseName = japaneseName;
			return this;
		}

		public SeiyuuBuilder WithAbout(string about)
		{
			_about = about;
			return this;
		}
		public SeiyuuBuilder WithModificationdDate(DateTime modificationDate)
		{
			_modificationDate = modificationDate;
			return this;
		}
	}
}