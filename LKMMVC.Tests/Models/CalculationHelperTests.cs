using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LKMMVC_1.Models;
using Xunit;

namespace LKMMVC.Tests
{
    public class CalculationHelperTests
    {
        [Theory]
        [InlineData("Aš einu namų link kur kelias drėgnas ir būgus žąsinas.","AS-EINU-NAMU-LINK-KUR-KELIAS-DREGNAS-IR-BUGUS-ZASINAS.")]
        [InlineData("aš esu [tas išrinktasis /kuris esu @4. hoho.", "AS-ESU-TAS-ISRINKTASIS-KURIS-ESU-4.-HOHO.")]
        public void ChangeNewsTitle_NewsTitleShouldChangeLithuanianSymbols(string title, string expected)
        {
            //Arrange
            
            //Act
            string actual = CalculationHelper.ChangeNewsTitle(title);
            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
