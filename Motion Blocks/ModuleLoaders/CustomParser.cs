using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;


namespace MotionBlocks.ModuleLoaders
{
	public static class CustomParser
	{
		public static float LenientTryParseFloat(JObject obj, string key, float defaultValue)
		{
			JToken jtoken;
			if (obj.TryGetValue(key, out jtoken))
			{
				if (jtoken.Type == JTokenType.Float)
				{
					return jtoken.ToObject<float>();
				}
				else if (jtoken.Type == JTokenType.Integer)
				{
					return (float)jtoken.ToObject<int>();
				}
				else if (jtoken.Type == JTokenType.String)
				{
					if (float.TryParse(jtoken.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out float parsed))
					{
						return parsed;
					}
				}
				else if (jtoken.Type == JTokenType.Boolean)
				{
					return jtoken.ToObject<bool>() ? 1.0f : 0.0f;
				}
			}
			return defaultValue;
		}

		public static bool LenientTryParseBool(JObject obj, string key, bool defaultValue)
		{
			JToken jtoken;
			if (obj.TryGetValue(key, out jtoken))
			{
				if (jtoken.Type == JTokenType.Float)
				{
					return jtoken.ToObject<float>() != 0.0f;
				}
				else if (jtoken.Type == JTokenType.Integer)
				{
					return jtoken.ToObject<int>() != 0;
				}
				else if (jtoken.Type == JTokenType.String)
				{
					if (float.TryParse(jtoken.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out float parsed))
					{
						return parsed != 0.0f;
					}
					else if (jtoken.ToString().ToLowerInvariant().Equals("false"))
					{
						return false;
					}
					return true;
				}
				else if (jtoken.Type == JTokenType.Boolean)
				{
					return jtoken.ToObject<bool>();
				}
			}
			return defaultValue;
		}

		public static Vector3 GetVector3(JToken token)
		{
			Vector3 result = Vector3.zero;
			if (token.Type == JTokenType.Object)
			{
				JObject jData = (JObject)token;

				if (jData.TryGetValue("x", out JToken xToken) && (xToken.Type == JTokenType.Integer || xToken.Type == JTokenType.Float))
				{
					result.x = xToken.ToObject<float>();

				}
				else if (jData.TryGetValue("X", out xToken) && (xToken.Type == JTokenType.Integer || xToken.Type == JTokenType.Float))
				{
					result.x = xToken.ToObject<float>();
				}

				if (jData.TryGetValue("y", out JToken yToken) && (yToken.Type == JTokenType.Integer || yToken.Type == JTokenType.Float))
				{
					result.y = yToken.ToObject<float>();

				}
				else if (jData.TryGetValue("Y", out yToken) && (yToken.Type == JTokenType.Integer || yToken.Type == JTokenType.Float))
				{
					result.y = yToken.ToObject<float>();
				}

				if (jData.TryGetValue("z", out JToken zToken) && (zToken.Type == JTokenType.Integer || zToken.Type == JTokenType.Float))
				{
					result.z = zToken.ToObject<float>();

				}
				else if (jData.TryGetValue("Z", out zToken) && (zToken.Type == JTokenType.Integer || zToken.Type == JTokenType.Float))
				{
					result.z = zToken.ToObject<float>();
				}
			}
			else if (token.Type == JTokenType.Array)
			{
				JArray jList = (JArray)token;
				for (int i = 0; i < Math.Min(3, jList.Count); i++)
				{
					switch (i)
					{
						case 0:
							result.x = jList[i].ToObject<float>();
							break;
						case 1:
							result.y = jList[i].ToObject<float>();
							break;
						case 2:
							result.z = jList[i].ToObject<float>();
							break;
					}
				}
			}
			return result;
		}

		public static Vector2 GetVector2(JToken token)
		{
			Vector2 result = Vector2.zero;
			if (token.Type == JTokenType.Object)
			{
				JObject jData = (JObject)token;

				if (jData.TryGetValue("x", out JToken xToken) && (xToken.Type == JTokenType.Integer || xToken.Type == JTokenType.Float))
				{
					result.x = xToken.ToObject<float>();

				}
				else if (jData.TryGetValue("X", out xToken) && (xToken.Type == JTokenType.Integer || xToken.Type == JTokenType.Float))
				{
					result.x = xToken.ToObject<float>();
				}

				if (jData.TryGetValue("y", out JToken yToken) && (yToken.Type == JTokenType.Integer || yToken.Type == JTokenType.Float))
				{
					result.y = yToken.ToObject<float>();

				}
				else if (jData.TryGetValue("Y", out yToken) && (yToken.Type == JTokenType.Integer || yToken.Type == JTokenType.Float))
				{
					result.y = yToken.ToObject<float>();
				}
			}
			else if (token.Type == JTokenType.Array)
			{
				JArray jList = (JArray)token;
				for (int i = 0; i < Math.Min(3, jList.Count); i++)
				{
					switch (i)
					{
						case 0:
							result.x = jList[i].ToObject<float>();
							break;
						case 1:
							result.y = jList[i].ToObject<float>();
							break;
					}
				}
			}
			return result;
		}

		public static Vector3 LenientTryParseVector3(JObject obj, string key, Vector3 defaultValue)
		{
			if (obj.TryGetValue(key, out JToken jtoken))
			{
				return GetVector3(jtoken);
			}
			return defaultValue;
		}

		public static Vector2 LenientTryParseVector2(JObject obj, string key, Vector2 defaultValue)
		{
			if (obj.TryGetValue(key, out JToken jtoken))
			{
				return GetVector2(jtoken);
			}
			return defaultValue;
		}
	}
}
