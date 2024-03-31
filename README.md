
## Welcome to the repository of the FvSocial backend!




![Logo](https://private-user-images.githubusercontent.com/94399375/318242221-4f553ee9-ccbd-41e4-8f44-6927c966666f.jpg?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3MTE4NjA4NjUsIm5iZiI6MTcxMTg2MDU2NSwicGF0aCI6Ii85NDM5OTM3NS8zMTgyNDIyMjEtNGY1NTNlZTktY2NiZC00MWU0LThmNDQtNjkyN2M5NjY2NjZmLmpwZz9YLUFtei1BbGdvcml0aG09QVdTNC1ITUFDLVNIQTI1NiZYLUFtei1DcmVkZW50aWFsPUFLSUFWQ09EWUxTQTUzUFFLNFpBJTJGMjAyNDAzMzElMkZ1cy1lYXN0LTElMkZzMyUyRmF3czRfcmVxdWVzdCZYLUFtei1EYXRlPTIwMjQwMzMxVDA0NDkyNVomWC1BbXotRXhwaXJlcz0zMDAmWC1BbXotU2lnbmF0dXJlPTc0NjU0YTE2MzJiM2NkMjAyY2FmNzNlYzUxOTE1N2FmZjhmNzk5YTU5YWUxNzExMDk0NTlmOWNkZGMxYzM2NjUmWC1BbXotU2lnbmVkSGVhZGVycz1ob3N0JmFjdG9yX2lkPTAma2V5X2lkPTAmcmVwb19pZD0wIn0.LLQmc4qHj2-XEzEHRsdNzbElX6Xqirne4lnShkTiqQU)



# FvSocial Backend
## Description
FvSocial is a project aimed at being a clone of Twitter with some modifications. This repository contains the backend of the project, which provides the logic and functionalities necessary for the application to work correctly.

## Architecture
### The FvSocial backend is structured as follows:

#### DataContext: Uses SQL Server as the database.
#### Entities: Contains the system's entities.
 
#### Controllers: Here are the controllers that manage HTTP requests and communicate with the services.
#### DTO: Data Transfer Objects of the entities, mapped with AutoMapper.

#### Services: Additionally, we have the Services folder, which houses the functions used by the controllers of each entity to make changes in the database.



## Contribution
Feel free to contribute to the project! You can open issues, submit pull requests, or suggest new features.

## Contact
If you have any questions or suggestions, feel free to contact us via email.

Thank you for contributing to the FvSocial project!


# Configuration
To ensure the backend works correctly, you need to configure some environment variables and external services:

In the appsettings.json file, add the following configuration for authentication:


Replace (your-secret-key) with your own secret key.


```javascript
"Authentication": {
  "SecretForKey": "(your-secret-key)"
}
```

If some endpoints use image creation, you'll need a Cloudinary account.

For Google authentication, add the client_id of your Google application.


## Demo
Frontend: https://github.com/Valenti-Franco/FVSocial
https://fvsocial.vercel.app/


![Captura de pantalla 2024-03-31 015711](https://github.com/Valenti-Franco/FVSocialAPI/assets/94399375/959b5522-2ca3-4803-bd37-f5885c5c6e0d)

