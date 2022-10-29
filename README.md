# Unidemo REST API
Tämä on OAuth2 JWT autentikointi ja autorisointi kokeilu.<br/>
Autorisointi on toteutettu JWT tokeniin sisällytetyllä UserRoles claim: illä ja controllereiden "AuthorizeRole" atribuuteilla.<br/>
# Business logiikkaa
Teacher ja Student tauluja ei ole kannassa, vaan toiminnot on toteutettu User/UserRole pohjalta.  Selvyyden ja roolihallinnan vuoksi
käyttäjien toiminnot on kuitenkin sijoitettu omiin, roolien mukaan nimettyihin controllerihinsa.
# API Dokumentointi
Kattava Swagger, Swashbuckle OpenApi dokumentaatio.
# Tekniikkaa
-MSSQL tietokanta<br/>
-Entity Framework Core 6 ORM<br/>
-ASP NET Core 6<br/>
-ASP NET Core.Identity.EntityFrameworkCore<br/>
-ASP NET Core.Authentication.JwtBearer<br/>
-AutoMapper<br/>
# Selviä puutteita
Koska tämä on pääasiassa OAuth2/JWT/Role/Automapper kokeilu;<br/>
-Data layer: iä ei ole eristetty repository pattern:illä yksikkötestausta varten.<br/>
-Controllerit ovat lihavia.<br/>
-DTO: t eivät ole loogisessa järjestyksessä
