using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;

namespace cPanelSharp
{
    public class Client
    {
        private string _username;
        private string _password;
        private string _accessHash;
        private string _host;
        private string _protocol;
        private int _port;

        public Client(string username, string host, string password = null, string accessHash = null, 
            bool ssl = true, bool cpanel = false)
        {
            _username = username;
            _host = host;

            if (password.IsEmpty() && accessHash.IsEmpty())
                throw new MissingCredentialsException();
            else if ((!password.IsEmpty() && !accessHash.IsEmpty()) || (cpanel && !accessHash.IsEmpty()))
                throw new InvalidCredentialsException();
            else if (!accessHash.IsEmpty())
            {
                accessHash = accessHash.Replace("\n", "");
                _accessHash = string.Format("WHM {0}:{1}", username, accessHash);
            }
            else if (!password.IsEmpty())
                _password = password;

            _protocol = ssl ? "https" : "http";

            _port = cpanel ? 2082 : 2086;
            if (ssl)
                _port = cpanel ? 2083 : 2087;
        }

        public string Api1(string module, string func, object user = null, object param = null)
        {
            throw new NotImplementedException();
        }

        public string Api2(string module, string func, object user = null, object param = null)
        {
            var parameters = ConstructParamDictFromObject(param);

            parameters.Add("cpanel_jsonapi_apiversion", "2");
            parameters.Add("cpanel_jsonapi_module", module);
            parameters.Add("cpanel_jsonapi_func", func);

            if (user != null)
                parameters.Add("cpanel_jsonapi_user", user.ToString());

            return ValidateAndCall(parameters);
        }

        private string ValidateAndCall(IDictionary<string, string> parameters)
        {
            var module = "";
            parameters.TryGetValue("cpanel_jsonapi_module", out module);

            var func = "";
            parameters.TryGetValue("cpanel_jsonapi_func", out func);

            if ((_port == 2086 || _port == 2087) && !parameters.ContainsKey("cpanel_jsonapi_user"))
                throw new InvalidParametersException("User parameter required");
            else if (module.IsEmpty())
                throw new InvalidParametersException("Module parameter required");
            else if (func.IsEmpty())
                throw new InvalidParametersException("Function parameter required");

            return Call("cpanel", parameters);
        }

        private string Call(string command, IDictionary<string, string> parameters)
        {
            var url = BuildUrl(command);

            var client = new RestClient(url);

            if (_accessHash.IsNotEmpty())
                client.Authenticator = new AccessHashAuthenticator(_username, _accessHash);
            else
                client.Authenticator = new HttpBasicAuthenticator(_username, _password);

            var request = new RestRequest(Method.GET);

            foreach (var entry in parameters)
                request.AddParameter(entry.Key, entry.Value);

            var response = client.Execute(request);

            return response.Content;
        }

        private string BuildUrl(string command)
        {
            return string.Format("{0}://{1}:{2}/json-api/{3}", _protocol, _host, _port, command);
        }

        private IDictionary<string, string> ConstructParamDictFromObject(object param)
        {
            if (param == null)
                return new Dictionary<string, string>();

            var paramDict = new Dictionary<string, string>();

            foreach (var prop in param.GetType().GetProperties())
                paramDict.Add(prop.Name, prop.GetValue(param).ToString());

            return paramDict;
        }
    }

    class AccessHashAuthenticator : IAuthenticator
    {
        private readonly string _authHeader;

        public AccessHashAuthenticator(string username, string accessHash)
        {
            _authHeader = string.Format("WHM {0}:{1}", username, accessHash);
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddParameter("Authorization", _authHeader, ParameterType.HttpHeader); 
        }
    }
}
