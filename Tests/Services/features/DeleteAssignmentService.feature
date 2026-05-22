Feature: Delete Assignment

  Scenario: Assignment does not exist
    Given an assignment id
    And the repository returns null for that id
    When I delete the assignment
    Then the result should be false

  Scenario: Assignment exists and is deleted successfully
    Given an assignment id
    And the repository returns an assignment for that id
    And removing the assignment succeeds
    When I delete the assignment
    Then the result should be true
    And images are deleted for the assignment
    And videos are deleted for the assignment
