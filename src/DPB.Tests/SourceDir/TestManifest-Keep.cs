﻿//this whole file will be kept (will not change anything)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPB.Tests.SourceDir
{
    public class TestManifestKeep
    {
        public TestManifestKeep()
        {
            //DPBMARK CYSOFT
            var tip = "this line will stay here in OutputDir while Conditions have CYSOFT keyword.";
            //DPBMARK_END

            //DPBMARK NotStay
            var tip2 = "this line will not stay here in OutputDir while Conditions don't have NotStay keyword.";
            //DPBMARK_END

            #region 配置和使用 Memcached      -- DPBMARK NotStay
            var tip3 = "this line will not stay here in OutputDir while Conditions don't have NotStay keyword.";
            #endregion                        -- DPBMARK_END

            var tip4 = "this line will not stay here in OutputDir while Conditions don't have NotStay keyword.";//DPBMARK NotStay DPBMARK_END

            var tip_stay = "this line will be always stay here.";
        }
    }
}
