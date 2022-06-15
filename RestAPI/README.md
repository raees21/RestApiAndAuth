# C-Sharp Level up
### main branch is the latest version of the API. Also, Lucky, we aren't changing anything based on your feedback ;)
## Workflow
Not started -> In progress -> Testing -> Promote to master -> Done
                                    
1. Select a task resolve under issues.
  - If this issue does not exist, create a new one with the **appropriate title**, **description**, and **label** (_feature_, _bugfix_, or _documentation_).
2. Inside the issue, use the '**Create merge request**' button to create a new branch off of '_development_' with the following naming convention:
  - prefix of either **bugfix** or **feature** depending on the issue label.
  - then the default naming by GitLab
  - eg: Issue #12 (Fetch single product variant) labeled feature: development -> **feature/12-fetch-single-product-variant**
3. Change label from **Not started** to **In progress**.
4. Once you are finished with development create a PR to development and try get one review.
5. When the PR is merged, change the label to **Testing**, delete the branch and test the feature/bugfix in the development branch
6. If the solution fails in development, create a new issue labelled **bugfix** and start from **step 1**.
7. A working solution in development means the label can be changed to **Promote to master**.
8. Periodically we will ensure that development is stable and PR from development to master. 
9. We can then confirm what issues were completed and done, and label and close them accordingly.

If shit in master is broken then we will use a hotfix process where a branch will be taken off master, labeled **hotfix/short-description** and merged straight into master. Once the fix is confirmed we will back propogate the change to development.
 
## Getting an access token to test the API
This API is secured by Bearer tokens to authenticate and authorize requests. To use any of the endpoints, generate a dev access token by using the following endpoint once the application is built and running.

### POST /api/v1/auth
### Payload:
{
  "userId": Guid,
  "type": "Buyer" or "Administrator"
}

- If you have content with a current userId then add that, if you need a new user leave this field out and new userId will be created.
- Choose either be Seller or Administrator roles that you wisht the user to take on.

### Response:
{
  "userId": Guid,
  "accessToken": "yourAccessTokenForAuthenticationAuthorization"
}
### Copy and paste the token in the reponse to the 'Authorization' function in the Swagger docs or add to the 'Authorization' header in all subsequent requests if using Postman or the like
