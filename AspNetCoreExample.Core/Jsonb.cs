namespace AspNetCoreExample
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    // TODO: Make Jsonb be capable to be used on DTOs itself, currently Jsonb can be used only on domain types.
    // Tried to create TypeConverter to make Web API able to serialize the JSON to this Jsonb type;
    // unfortunately, it cannot be called for some reasons.
    // Many have same experience: https://www.google.com.ph/search?q=typeconverter+not+called
    // So for now, let's just use JSON.NET's JObject type for DTOs.
    // The TODO might be hard. Let's just do this in the future.
    // An example of DTO that uses JObject (instead of Jsonb):
    // AspNetCoreExample.Dto.AspNetCoreExampleDto.AspNetCoreExample.Persist.AspNetCoreExampleModuleDto:    
    /*
    public class AspNetCoreExampleModuleDto
    {
        public int       AspNetCoreExampleModuleId { get; set; }
        public JObject   Saved           { get; set; }   
    }
    */

    public struct Jsonb
    {
        readonly string _value;


        Jsonb(string jsonString) => _value = jsonString;

        Jsonb(string key, object value) =>
            _value = JsonConvert.SerializeObject(new Dictionary<string, object> { { key, value } });


        public static bool operator ==(Jsonb a, Jsonb b)
        {
            if (System.Object.ReferenceEquals(a, b))
                return true;

            bool areEqual = a.Equals(b);

            return areEqual;
        }

        public static bool operator !=(Jsonb a, Jsonb b) => !(a == b);

        public override bool Equals(object obj)
        {
            Jsonb a = (Jsonb)this;
            Jsonb b = (Jsonb)obj;

            if (a._value == null && b._value == null)
                return true;

            if (a._value == null || b._value == null)
                return false;

            bool areEqual = this._value.Equals(((Jsonb)obj)._value);

            return areEqual;
        }

        public override int GetHashCode() => this._value.GetHashCode();

        public override string ToString() => this._value;

        public static Jsonb Create(string json) => json == "{}" ? Jsonb.Empty : new Jsonb(json);

        public static Jsonb Create(string key, object value) => new Jsonb(key, value);

        public static Jsonb Merge(Jsonb jsonX, Jsonb jsonY)
        {
            if (jsonX == Jsonb.Null && jsonY != Jsonb.Null)
                return jsonY;

            if (jsonY == Jsonb.Null && jsonX != Jsonb.Null)
                return jsonX;

            if (jsonX == Jsonb.Null && jsonY == Jsonb.Null)
                return Jsonb.Null;

            JObject toMergeObjectA = JObject.Parse(jsonX.ToString());

            JObject toMergeObjectB = JObject.Parse(jsonY.ToString());

            toMergeObjectA.Merge(
                toMergeObjectB,
                new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union }
            );

            return Jsonb.Create(toMergeObjectA.ToString());
        }


        // TODO: SQL backing. Might not be needed though.
        // Compared to Localize method, there's no use case yet that this MergeLocalization 
        // will be used in SELECT, WHERE nor ORDER BY. 
        // Whereas .Localize should be SQL generated, that is it can be used on 
        // ORDER BY, SELECT, WHERE, so the Linq use of .Localize method should have 
        // an SQL equivalent, so the Linq will be ran at the database level.
        // Jsonb's Merge method is an application-level thing only, for now.
        public Jsonb Merge(string lang, string text) => Jsonb.Merge(this, Jsonb.Create(lang, text));           

        public static readonly Jsonb Null = new Jsonb(null);

        public static readonly Jsonb Empty = new Jsonb("{}");

    }


    // This method best belong to Jsonb class itself. 
    // However, due to the infrastructure code for integrating SQL function to NHibernate, 
    // we have to do Jsonb's GetStringValue method as extension method, applies to Localize method too.
    // 
    // The infrastructure code requires extension method: http://www.primordialcode.com/blog/post/nhibernate-customize-linq-provider-user-defined-sql-functions/
    // Tried the normal method instead of extension method, but the signature of jsonb_extract_path_text resolves to this only:
    //      jsonb_extract_path_text(text)
    // That is, it's only single parameter, instead of the correct two parameters:
    //      jsonb_extract_path_text(jsonb, text)
    public static class JsonbExtension
    {
        public static string GetStringValue(this Jsonb jsonb, string key)
        {
            // We can also do: if (jsonb.ToString() != null)
            // But it's better to abstract things out to future-proof things, 
            // we might want to use blank instead of null in the future.

            if (jsonb != Jsonb.Null)
            {
                var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonb.ToString());
                if (d.ContainsKey(key))
                    return (string)d[key];
                else
                    return null;
            }
            else
            {
                return null;
            }

        }

        public static string Localize(this Jsonb jsonb, string lang, string fallbackLang) =>
            jsonb.GetStringValue(lang) ?? jsonb.GetStringValue(fallbackLang);
    }

}
