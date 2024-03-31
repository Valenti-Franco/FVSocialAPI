FvSocial Backend Readme
Welcome to the repository of the FvSocial backend!

Description
FvSocial is a project aimed at being a clone of Twitter with some modifications. This repository contains the backend of the project, which provides the logic and functionalities necessary for the application to work correctly.

Architecture
The FvSocial backend is structured as follows:

DataContext: Uses SQL Server as the database.
Entities: Contains the system's entities.
Controllers: Here are the controllers that manage HTTP requests and communicate with the services.
DTO: Data Transfer Objects of the entities, mapped with AutoMapper.
Additionally, we have the Services folder, which houses the functions used by the controllers of each entity to make changes in the database.

Configuration
To ensure the backend works correctly, you need to configure some environment variables and external services:

In the appsettings.json file, add the following configuration for authentication:

json
Copy code
"Authentication": {
  "SecretForKey": "(your-secret-key)"
}
Replace (your-secret-key) with your own secret key.

If some endpoints use image creation, you'll need a Cloudinary account.

For Google authentication, add the client_id of your Google application.

Contribution
Feel free to contribute to the project! You can open issues, submit pull requests, or suggest new features.

Contact
If you have any questions or suggestions, feel free to contact us via email.

Thank you for contributing to the FvSocial project!
