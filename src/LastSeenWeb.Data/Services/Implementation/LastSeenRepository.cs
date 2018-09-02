﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using LastSeenWeb.Data.Models;
using LastSeenWeb.Domain.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LastSeenWeb.Data.Services.Implementation
{
	public class LastSeenRepository : ILastSeenRepository
	{
		private const string DatabaseName = "mo10097_lastseen";
		private const string CollectionName = "LastSeenItems";

		private readonly IMapper _mapper;
		private readonly IMongoClient _mongoClient;
		private readonly ILogger<LastSeenRepository> _logger;

		public LastSeenRepository(IMapper mapper, IMongoClient mongoClient, ILogger<LastSeenRepository> logger)
		{
			_mapper = mapper;
			_mongoClient = mongoClient;
			_logger = logger;
		}

		public async Task<List<LastSeenItem>> GetAll(string ownerName)
		{
			try
			{
				var db = _mongoClient.GetDatabase(DatabaseName);
				var lastSeenItems = db.GetCollection<LastSeenItemEntity>(CollectionName);
				var result = await lastSeenItems
					.Find(e => e.OwnerName == ownerName)
					.SortByDescending(e => e.Modified)
					.ToListAsync();

				return _mapper.Map<List<LastSeenItem>>(result);
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
			}

			return new List<LastSeenItem>();
		}

		public async Task<LastSeenItem> Get(string id, string ownerName)
		{
			try
			{
				var db = _mongoClient.GetDatabase(DatabaseName);
				var lastSeenItems = db.GetCollection<LastSeenItemEntity>(CollectionName);
				var result = await lastSeenItems
					.Find(e => e.Id == id && e.OwnerName == ownerName)
					.FirstOrDefaultAsync();

				return _mapper.Map<LastSeenItem>(result);
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
			}

			return null;
		}

		public async Task Upsert(LastSeenItem lastSeenItem, string ownerName)
		{
			try
			{
				var db = _mongoClient.GetDatabase(DatabaseName);
				var lastSeenItems = db.GetCollection<LastSeenItemEntity>(CollectionName);

				var entity = _mapper.Map<LastSeenItemEntity>(lastSeenItem);
				entity.OwnerName = ownerName;
				entity.Modified = DateTime.UtcNow;

				if (string.IsNullOrWhiteSpace(entity.Id) || entity.Id == "undefined")
				{
					entity.Id = null;
					await lastSeenItems.InsertOneAsync(entity);
				}
				else
				{
					await lastSeenItems.ReplaceOneAsync(e => e.Id == entity.Id, entity);
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
			}
		}

		public async Task Delete(string id, string ownerName)
		{
			try
			{
				var db = _mongoClient.GetDatabase(DatabaseName);
				var lastSeenItems = db.GetCollection<LastSeenItemEntity>(CollectionName);
				await lastSeenItems.DeleteOneAsync(e => e.Id == id && e.OwnerName == ownerName);
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
			}
		}
	}
}
