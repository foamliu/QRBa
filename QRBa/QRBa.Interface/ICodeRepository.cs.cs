using QRBa.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRBa.Interface
{
    public interface ICodeRepository
    {
        Code AddCode(Code newCode);

        Code UpdateCode(Code newCode);

        Code GetCode(int accountId, int codeId);

        List<Code> GetCodes(int accountId);
    }
}
