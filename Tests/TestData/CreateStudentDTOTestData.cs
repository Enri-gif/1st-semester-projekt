using api.DTOs;
using api.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Tests.TestData;

public class CreateStudentDTOTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { new CreateStudentDTO { FirstName = "Jimmy", LastName = "Joe Jim-bob"} };
        yield return new object[] { new CreateStudentDTO { FirstName = "Derek", LastName = "Derpson"} };
        yield return new object[] { new CreateStudentDTO { FirstName = "Allan", LastName = "Astridsson"} };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
