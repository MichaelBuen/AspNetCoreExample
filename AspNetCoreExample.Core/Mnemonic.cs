using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreExample
{
    // We don't need to use the enum's values,
    // this is just created to give an early warning system if a translation is already created.
    // The names are intentionally camelCase, making it javascript naming-convention conforming.
    // This is used for _core.translation's mnemonic column in Fluent Migrations
    // Typescript equivalent: 
    // \AspNetCoreExample\src\AspNetCoreExample.Admin\TypeScript-Equivalent\Constant\I18n.ts
    public enum Mnemonic
    {
        AspNetCoreExample,

        // It's better to distinguish non-verbatim mnemonic to verbatim ones using a convention.
        // To distinguish non-verbatim mnemonic from verbatim mnemonic, prefix an underscore.
        // Not having a convention for non-verbatim mnemonic makes its name very long, as we tend to avoid name collission from verbatim mnemonic.
        // Not having a convention for non-verbatim mnemonic makes coming up with a good name harder too.
        // Starting today, May 16, all non-verbatim mnemonics by convention has underscore prefix.


        // segregated non-verbatim mnemonic
        noteModuleImagesShouldBe, // given text: Note: Module images should be 250px by 250px
        clonePrompt                              , // given text: This will clone the Module information including its fields and fields options
        cloneBlankWarning                        , // given text: Input the new module's name
        cloneSameNameWarning                     , // given text: Use different module name.
        clientBlankWarning                       , // given text: Input the client name.
        moduleCompletenessConfirmation           , // given text: This information is complete and correct to the best of my knowledge       
        duplicateTemplateNameConstraint          , // given text: Duplicate Template Name. Give a different name.
        errorApology                             , // given text: We apologize but an expected error occur.
        mustUploadNewTemplateConstraint          , // given text: Must upload new template
        notAllowedToAccessOtherTenantData        , // given text: Not allowed to access other tenant's data
        publicTemplateNote                       , // given text: Italicized are public templates
        moonlightHasRegisteredUserNote             , // given text: Cannot register new user to this AspNetCoreExample session. This AspNetCoreExample session is already registered to a user
        blankSessionWarning                      , // given text: This might not be your AspNetCoreExample record, or AspNetCoreExample session was cancelled
        moonlightAlreadyCompletedNotify            , // given text: You already completed the AspNetCoreExample. Thanks for checking.
        moonlightNotYetCompletmoonlighty             , // given text: You haven't completed the AspNetCoreExample. You can resume it anytime.
        moonlightCompleteThanksNotify              , // given text: Thanks for completing the AspNetCoreExample.
        emailAlreadyRegisteredNotify             , // given text: Your email is already registered to your existing username. Log in with your existing username or have your password reset.
        cannotSelfmoonlightCreatedApplicantNotify  , // given text: You cannot moonlight the applicant you registered.        
        failedCreatePasswordChangeSessionNotify  , // given text: Cannot do a password reset, no existing account yet. Please register first.        
        passwordChangeSessionInvalidNotify       , // given text: Password Change session is already invalid
        accountCreatedNotify                     , // given text: Account Created. You will be directed to login.

        passwordResetEmailSubject                , // given text: Password reset request
        passwordResetEmailBody                   , // given text: You requested a <a href="https://{subdomain}.{domain}/password-changer/{userId}/{passwordChangeSession}">password reset</a>

        moonlightEmailSubject                      , // given text: Welcome {firstName} to {companyName}
        moonlightEmailBody                         , // given text: You can now <a href="https://{subdomain}.{domain}/moonlight/{moonlightId}/{moonlightSession}">start</a> your company AspNetCoreExample

        employeemoonlightCompletionRequirementNotify, // given text: You can do the review after the employee completed his/her AspNetCoreExample.

        // ec = Employee Completed
        ecmoonlightEmailSubject                     , // given text: {firstName} {middleName} {lastName} completed the AspNetCoreExample
        ecmoonlightEmailBody                        , // given text: Next step is do the manager's <a href='https://{subdomain}.{domain}/moonlight/{moonlightId}/{moonlightSession}?openedByManager=true'>AspNetCoreExample part</a>. Anytime, you can <a href="https://{subdomain}.{domain}.com/configuration/moonlight/manager">review all</a> the applicants who already completed their AspNetCoreExample.

        passwordsMustBeSameNotify                 , // given text: Password and confirmation password must be same
        passwordRequirementNotify                 , // given text: Password must not be empty. It must be greater than 6 characters. 

        _fieldMention                              , // given text: Use @ to load the field list. Similar to Facebook's mention.        
        _passwordRequirement                       , // given text: Minimum 8 characters at least 1 Alphabet, 1 Number and 1 Special Character
        _appName                                   , // given text: Simple moonlight
        _checkInputs                               , // given text: Please check your input
        _isoAlpha2CodeTaken                        , // given text: ISO Alpha 2 Code is already taken. Please Change. 
        _isoAlpha3CodeTaken                        , // given text: ISO Alpha 3 Code is already taken, Please Change 
        _countryNameTaken                          , // given text: Country name is already taken. Please Change.
        _geoRegionNameTaken                        , // given text: Geographical Region name is already taken. Please Change.        
        _selCompany                                , // given text: Select a company
        _companyAdmin                              , // given text: Company Administrator
    }
}


/*
 * 
 * Log with the username associated with that email.
 * For password reset, go here: http://wwww.AspNetCoreExample.com/password-reset
 * 
 */