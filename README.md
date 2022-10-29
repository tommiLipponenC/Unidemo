# Unidemo REST API
Tämä on OAuth2 JWT autentikointi ja autorisointi kokeilu.<br/>
Autorisointi on toteutettu JWT tokeniin sisällytetyllä UserRoles claim: illä ja controllereiden "AuthorizeRole" atribuuteilla.<br/>
# Business logiikkaa
Teacher ja Student tauluja ei ole kannassa, vaan toiminnot on toteutettu User/UserRole pohjalta.  Selvyyden ja roolihallinnan vuoksi
käyttäjien toiminnot on kuitenkin sijoitettu omiin, roolien mukaan nimettyihin controllerihinsa.
# API Dokumentointi
Kattava Swagger, Swashbuckle OpenApi dokumentaatio.
# Tekniikkaa
-MSSQL tietokanta
-Entity Framework Core 6 ORM
