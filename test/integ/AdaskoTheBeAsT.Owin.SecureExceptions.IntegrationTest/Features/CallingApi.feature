Feature: CallingApi

Calling api

Background:
    Given I have proper client

Scenario: Calling proper api with success result
    When I call api "api/sample"
    Then I should get success result


Scenario: Calling non existent api with failure result and sanitized error message
    When I call non existent api "api/nonexistent"
    Then I should get error with message '{"Message":"No HTTP resource was found that matches the request URI.","MessageDetail":null}'

Scenario Outline: Calling api with some script injection
    When I call api with malicious url "<sampleUrl>"
    Then I should get error with message '<expected>'

Examples:
    | sampleUrl                                                               | expected                                                                                   |
    | /api/values?query=<script>alert('xss')</script>                         | {"Message":"No HTTP resource was found that matches the request URI.","MessageDetail":null} |
    | /api/values/%2522%253e%253cscript%253ealert('xss')%253c%252fscript%253e | "Error"                                                                                    |
    | /api/values?query="><script>alert('xss')</script>                       | {"Message":"No HTTP resource was found that matches the request URI.","MessageDetail":null} |
    | /api/values?query=%22onmouseover%3d%22alert('xss')%22                   | {"Message":"No HTTP resource was found that matches the request URI.","MessageDetail":null} |
    | /api/values?query=';DROP TABLE Users;--                                 | {"Message":"No HTTP resource was found that matches the request URI.","MessageDetail":null} |
