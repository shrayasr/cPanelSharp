# cPanelSharp

A simple C# wrapper for accessing cPanel APIs. 

## Caveats

- Currently the API returns the json in a string format. The user is expected to define the object and deserialize using a json deserializer ([json.net](http://www.newtonsoft.com/json), [Jil](https://github.com/kevin-montrose/Jil)).
- Only API v2 supported

## Usage

First, we declare a `cPanelClient` with required params.

To connect to a cPanel instance, the required parameters are: 

- username
- hostname (will be something like `*.sgcpanel.com`)
- password

also the `cPanel` argument needs to be set to `true`.

```C#
var client = new cPanelClient("<username>", "<hostname>", password: "<password>", cPanel: true);
```

Look up the [cPanel API 2](https://documentation.cpanel.net/display/SDK/Guide+to+cPanel+API+2) and find the call you want to make.

Consider that you want to get the [list of email accounts](https://documentation.cpanel.net/display/SDK/cPanel+API+2+Functions+-+Email%3A%3Alistpops). You'll see `Email::listpops` on the title. Everything to the left of `::` forms the `module` and everything to the right of `::` forms the `function`. With this we get:

- Module: `Email`
- Function: `listpops`

Now, we can call the `Api2` function with the required parameters

```C#
var response = client.Api2("Email", "listpops");
```

Parameters can be specified using an object. `Email::listpops` takes an optional `regex` param to search by. Lets pass that to the function in the form of an object

```C#
var response = client.Api2("Email", "listpops", new 
{
  regex = "foo"
});
```

## Credits

All credits to https://github.com/vexxhost/python-cpanelapi
