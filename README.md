AspNetCoreExample Backend
======================

This handles **Identity** web app and **REST API** services


Coding guideline
------------------

Add your code approaches rationale here.


### Guideline on private: ###

Have same opinion as Miguel de Icaza on private keyword. Miguel de Icaza creator of open source C#



> "@JeffRatcliff @devlead @gblock it is unnecessary typing, busy work, noise.  
    It is not just variables, it applies to all members and types" -- https://twitter.com/migueldeicaza/status/533743674904289281


>   ". @gblock 'private' is the herpes of c#: serves no purpose, is infectious and makes code look bad.  
    Best reason for it is 'everyone has it'" -- https://twitter.com/migueldeicaza/status/533736372642017280

            
>  "@JakeGinnivan @gblock different.  Internal has an effect.  
    Private is a placebo, the compiler just ignores it" -- https://twitter.com/migueldeicaza/status/533738227669426176            



### Guideline on Set v Bag: ###

Advisory for many-to-many, ***DO NOT USE Bags***, use Set instead. 

* Bag is dangerous for many-to-many as it's set to this in automapper, note the Delete:
    
    ```csharp
    propertyCustomizer.Cascade(
        NHibernate.Mapping.ByCode.Cascade.All | NHibernate.Mapping.ByCode.Cascade.DeleteOrphans
    );
    ```

* With the Bag automapping configuration above, deletions on many to many table also 
  deletes the referenced table.

* Example of dangerous, if Tenant's Users is a Bag collection and then a tenant is deleted, 
  not only the many-to-many entry of tenant's User in UserTenant table will be deleted,
  the User itself will be deleted too.

* Bag only makes sense for one-to-many, deleting aggregate root deletes the whole aggregate.
