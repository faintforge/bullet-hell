using OpenTK.Graphics.OpenGL;
using TKMatrix4 = OpenTK.Mathematics.Matrix4;
using System.Runtime.InteropServices;

namespace BulletHell {
    public class Shader {
        private int program = 0;

        ~Shader() {
            GL.DeleteProgram(program);
        }

        /// <summary>
        /// Create a shader from GLSL source strings.
        /// </summary>
        /// <param name="vertexSource">Vertex shader source code.</param>
        /// <param name="fragmentSource">Fragment shader source code.</param>
        /// <returns>Shader.</returns>
        public static Shader FromSource(string vertexSource, string fragmentSource) {
            Shader shader = new Shader();
            int success;

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            GL.CompileShader(vertexShader);
            GL.GetShaderi(vertexShader, ShaderParameterName.CompileStatus, out success);
            if (success == 0) {
                GL.GetShaderInfoLog(vertexShader, out string infoLog);
                Console.WriteLine("Vertex shader compilation error:");
                Console.WriteLine(infoLog);
            }

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            GL.CompileShader(fragmentShader);
            GL.GetShaderi(fragmentShader, ShaderParameterName.CompileStatus, out success);
            if (success == 0) {
                GL.GetShaderInfoLog(fragmentShader, out string infoLog);
                Console.WriteLine("Fragment shader compilation error:");
                Console.WriteLine(infoLog);
            }

            shader.program = GL.CreateProgram();
            GL.AttachShader(shader.program, vertexShader);
            GL.AttachShader(shader.program, fragmentShader);
            GL.LinkProgram(shader.program);
            GL.GetProgrami(shader.program, ProgramProperty.LinkStatus, out success);
            if (success == 0) {
                GL.GetProgramInfoLog(shader.program, out string infoLog);
                Console.WriteLine("Shader linking error:");
                Console.WriteLine(infoLog);
            }

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return shader;
        }

        /// <summary>
        /// Create a shader from shader files.
        /// </summary>
        /// <param name="vertexFilepath">Path to vertex shader.</param>
        /// <param name="fragmentFilepath">Path to fragment shader.</param>
        /// <returns>Shader.</returns>
        public static Shader FromFile(string vertexFilepath, string fragmentFilepath) {
            string vertexSource = File.ReadAllText(vertexFilepath);
            string fragmentSource = File.ReadAllText(fragmentFilepath);
            return Shader.FromSource(vertexSource, fragmentSource);
        }

        /// <summary>
        /// Bind this shader for drawing.
        /// </summary>
        public void Use() {
            GL.UseProgram(program);
        }

        /// <summary>
        /// Send a 4x4 matrix as a uniform to the shader. 
        /// </summary>
        /// <param name="name">Uniform location name.</param>
        /// <param name="matrix">Matix value.</param>
        public void UniformMatrix4(string name, Matrix4 matrix) {
            int loc = GL.GetUniformLocation(program, name);
            TKMatrix4 tkMatrix = new TKMatrix4(
                    matrix.I.X,
                    matrix.I.Y,
                    matrix.I.Z,
                    matrix.I.W,
                    matrix.J.X,
                    matrix.J.Y,
                    matrix.J.Z,
                    matrix.J.W,
                    matrix.K.X,
                    matrix.K.Y,
                    matrix.K.Z,
                    matrix.K.W,
                    matrix.L.X,
                    matrix.L.Y,
                    matrix.L.Z,
                    matrix.L.W
                );
            GL.UniformMatrix4f(loc, 1, false, ref tkMatrix);
        }

        /// <summary>
        /// Send an integer as a uniform to the shader.
        /// </summary>
        /// <param name="name">Uniform location name.</param>
        /// <param name="value">Integer value.</param>
        public void UniformInt(string name, int value) {
            int loc = GL.GetUniformLocation(program, name);
            GL.Uniform1i(loc, value);
        }

        /// <summary>
        /// Send an integer array as a uniform to the shader.
        /// </summary>
        /// <param name="name">Uniform location name.</param>
        /// <param name="array">Integer array.</param>
        public void UniformInt(string name, int[] array) {
            int loc = GL.GetUniformLocation(program, name);
            GL.Uniform1i(loc, array.Length, array);
        }
    }
}
