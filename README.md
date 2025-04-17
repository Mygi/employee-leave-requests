# Vypex Coding Challenge

## Problem Statement

HR require a portal to manage employees and their leave days. The portal should be able to display a list of employees, edit an employee's leave, add leave days, and show the total leave days taken by an employee on the employee list.

## Requirements

The portal requires the following features:

* Displays the list of employees.
* Can add and remove leave days.
* Shows the total leave days taken by an employee on the employee list.
* Leave days must not overlap.
* Leave days can be edited.
* Leave days have a start and end date. No need to track leave hours.
* Filter employees by name on the employee list.

## What's provided

You're inheriting a partially completed project. The authors may or may not have been a bit sleepy after Thursday's steak special team lunch at the Carlton Brewhouse.

### Backend

* Scaffolded ASP.NET Core 9 Web API with an endpoint that returns a mock list of employees.
* Pre populated SQLite database that is currently NOT connected to the endpoint.
* Basic Employee model class with Id and Name properties registered in EF Core.
* By default the service is available at https://localhost:7189
	* OpenAPI document https://localhost:7189/openapi/v1.json
	* API browser https://localhost:7189/scalar/v1
* `dotnet ef` tool has been installed as part of the project which can be run from the `Vypex.CodingChallenge.Service` folder.

### Frontend

* Scaffolded Angular 19 frontend.
* An API service that communicates with the backend.
* Front end with employees dashboard that lists employees.
* NgZorro/Ant Design for UI components.

## What we want to see

We like to keep up with the latest and greatest from the Dotnet and Angular worlds so please use the latest technologies and best practices.
If there is anything in the scaffolded projects that you think could be done better please comment!

### Backend

* Connect the Employees endpoint to the "real" database.
* Add leave days to the employee model and database.
* Implement the leave days api in the backend.

### Frontend

#### Improve employees list component
* Add a refresh/reload button to refetch the list of employees.
* Use new Angular 19 resources.
* Handle potential API errors.
* Implement search by employee name functionality
	* Minimise the number of requests to the API where possible
* Feel free to change layout and structure of the page.

#### Implement Edit employee functionality
* Use Angular forms and Ant Design components for user input.
* Implement employee leave form control as a separate and re-usable component.
    * The user can dynamically add/modify multiple leave entries for an employee.
    * Leave days cannot overlap.
    * Both start and end date are required.
    * Validate that `startDate` must be before `endDate`.

#### Considerations
* Use [Ant Design](https://ng.ant.design/components/overview/en) UI library.
  * Feel free to add styles for any components you may use into */src/styles/antd.less* if they're not already included.
* Follow best coding practices and principles where you can.
* Incorporate latest features from Angular including [Angular 19](https://angular.love/angular-19-whats-new) where possible.
* Feel free to add new dependencies if needed.
* Unit tests are not required.

## Out of scope

* Editing employee properties other than leave days.
* Tests are not required.

## Tips

If you want to regenerate the SQLite db use the following command. The `--startup-project` and `--project` properties are important because of the project structure.
`dotnet ef database update --startup-project .\Vypex.CodingChallenge.Service\Vypex.CodingChallenge.Service.csproj --project .\Vypex.CodingChallenge.Infrastructure\Vypex.CodingChallenge.Infrastructure.csproj`

## Submission
Once you've completed the test, create a zip archive of your submission and send it to the recruiter.

Thank you and good luck!
