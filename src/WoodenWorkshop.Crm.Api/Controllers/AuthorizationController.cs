using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WoodenWorkshop.Crm.Api.Controllers;

[AllowAnonymous, Route("auth")]
public class AuthorizationController : ControllerBase
{
}