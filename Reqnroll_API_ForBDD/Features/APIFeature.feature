Feature: API

  Scenario: Call API endpoint
    When I call GET /req-test
    Then the response is Hello from ReqTest.