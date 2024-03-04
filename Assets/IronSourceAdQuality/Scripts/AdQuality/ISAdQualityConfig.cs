using System;
using UnityEngine;

public class ISAdQualityConfig
{
	private String userId;
	private bool userIdSet;
	private bool testMode;
	private ISAdQualityInitCallback adQualityInitCallback;
	private ISAdQualityLogLevel logLevel;

	public ISAdQualityConfig()
	{
		userId = null;
		testMode = false;
		userIdSet = false;
		logLevel = ISAdQualityLogLevel.INFO;
	}

	public String UserId 
	{
		get
		{
			return userId;
		}
		set 
		{
			userIdSet = true;
			userId = value;
		}
	}
	
	internal bool UserIdSet 
	{
		get 
		{
			return userIdSet;
		}	
	}
	
	public bool TestMode 
	{
		get
		{
			return testMode;
		} 
		set 
		{
			testMode = value;
		}
	}

	public ISAdQualityLogLevel LogLevel
	{
		get 
		{
			return logLevel;
		}
		set
		{
			logLevel = value;
		}
	}

	public ISAdQualityInitCallback AdQualityInitCallback 
	{
		get 
		{
			return adQualityInitCallback;
		} 
		set 
		{
			adQualityInitCallback = value;
		}
	}
}