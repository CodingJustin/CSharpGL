﻿using CSharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridViewer
{
    public partial class CatesianGridRenderer : GridViewRenderer
    {
        private NewTexture codedColorSampler;

        public static CatesianGridRenderer Create(CatesianGrid grid, NewTexture codedColorSampler)
        {
            var shaderCodes = new ShaderCode[2];
            shaderCodes[0] = new ShaderCode(File.ReadAllText(@"shaders\HexahedronGrid.vert"), ShaderType.VertexShader);
            shaderCodes[1] = new ShaderCode(File.ReadAllText(@"shaders\HexahedronGrid.frag"), ShaderType.FragmentShader);
            var map = new PropertyNameMap();
            map.Add("in_Position", CatesianGrid.strPosition);
            map.Add("in_uv", CatesianGrid.strColor);
            var renderer = new CatesianGridRenderer(grid, shaderCodes, map, codedColorSampler);
            return renderer;
        }

        private CatesianGridRenderer(CatesianGrid catesianGrid, ShaderCode[] shaderCodes,
            PropertyNameMap propertyNameMap, NewTexture codedColorSampler, params GLSwitch[] switches)
            : base(catesianGrid, shaderCodes, propertyNameMap, switches)
        {
            this.codedColorSampler = codedColorSampler;
        }


        protected override void DoInitialize()
        {
            base.DoInitialize();

            this.SetUniform("colorCodeSampler", new samplerValue(
                BindTextureTarget.Texture1D, this.codedColorSampler.Id, OpenGL.GL_TEXTURE0));
        }

        protected override void DoRender(RenderEventArg arg)
        {
            this.SetUniform("projectionMatrix", arg.Camera.GetProjectionMat4());
            this.SetUniform("viewMatrix", arg.Camera.GetViewMat4());
            this.SetUniform("modelMatrix", this.ModelMatrix);

            base.DoRender(arg);
        }
    }
}
