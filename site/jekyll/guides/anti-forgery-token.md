# Anti-Forgery Token
The anti-forgery token can be used to help protect your application against cross-site request forgery. 
To use this feature, call the AntiForgeryToken method from a form and add the ValidateAntiForgeryTokenAttribute 
attribute to the action method that you want to protect.

For genuine ASP.NET Applications with cshtml you will use `@Html.AntiForgeryToken()` to create the required input tag. 
With ASP.NET Core the required input tag will be injected for every form automatically. 

But, both scenarios are not 
applicable to react apps, neither for server-side nor for client-side rendering. Therefore, we need to pass the token 
as property from the server to the client. Than, it can 
be used for several scenarios like, form data posts, fetch and/or ajax requests. The token can be aquired from 
`IAntiforgery.GetAndStoreToken`.

## Sample

Assume we have a method with `ValidateAntiForgeryAttribute`:
```cs
public class AccountController : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginInputModel model)
    {
      /* Login Logic */
    }
}
```

Use `IAntiforgery ` within the `Index.cshtml` file. This will also set http-only cookie required for antiforgery validation.
```cshtml
@model HomeViewModel

@inject IAntiforgery Antiforgery
@Html.React("Home", new
{
    antiforgeryToken = Antiforgery.GetAndStoreTokens(Context).RequestToken
})
```

Now you can use the token within your react app and pass it either as form data or Header for fetch requests:
```jsx
// Use as form data input
function Home({ antiforgeryToken }) {
  return (
    <form action="/Account/Login" method="post">
       <input type="hidden" name="__RequestVerificationToken" value={antiforgeryToken} />
       {/* More form input */}
    </form>
  )
}

// Or use as Header for fetch
class Home extends React.Component {
  onPostSomething() {
    fetch('https://my-web.com/Account/Login', {
      method: 'POST',
      headers: {
        'RequestVerificationToken': this.props.antiforgeryToken,
        /* more headers */
      },
      body: {/* formdata or json or whatelse */}
    }).then( /* continue ... */ )
  }
}
```
