using OpenTK.Graphics.OpenGL;
using TKMatrix4 = OpenTK.Mathematics.Matrix4;

namespace BulletHell {
    public class Shader {
        private int program = 0;

        ~Shader() {
            GL.DeleteProgram(program);
        }

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

        public static Shader FromFile(string vertexFilepath, string fragmentFilepath) {
            string vertexSource = File.ReadAllText(vertexFilepath);
            string fragmentSource = File.ReadAllText(fragmentFilepath);
            return Shader.FromSource(vertexSource, fragmentSource);
        }

        public void Use() {
            GL.UseProgram(program);
        }

        public void UniformMatrix4(string name, Matrix4 matrix) {
            int loc = GL.GetUniformLocation(program, name);
            TKMatrix4 tkMatrix = new TKMatrix4(
                    matrix.I[0],
                    matrix.I[1],
                    matrix.I[2],
                    matrix.I[3],
                    matrix.J[0],
                    matrix.J[1],
                    matrix.J[2],
                    matrix.J[3],
                    matrix.K[0],
                    matrix.K[1],
                    matrix.K[2],
                    matrix.K[3],
                    matrix.L[0],
                    matrix.L[1],
                    matrix.L[2],
                    matrix.L[3]
                );
            GL.UniformMatrix4f(loc, 1, false, ref tkMatrix);
        }
    }
}
