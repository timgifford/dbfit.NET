require 'ProjectFile'
require 'test/unit'

class TestableProjectFile < ProjectFile
    def initialize(project_file)
        self.projectFile = project_file
        self.projectXml = Document.new(project_file)
    end
end

class ProjectFileTests < Test::Unit::TestCase

    def ProjectFileTests.ProjectSource
     %q!
        <Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
          <PropertyGroup>
            <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
            <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
            <ProductVersion>8.0.50727</ProductVersion>
            <SchemaVersion>2.0</SchemaVersion>
            <ProjectGuid>{5AAF67C1-BBEF-47E0-ABEC-B82B93C5A68F}</ProjectGuid>
            <OutputType>Library</OutputType>
            <AppDesignerFolder>Properties</AppDesignerFolder>
            <RootNamespace>Example.Startup</RootNamespace>
            <AssemblyName>Example.Startup</AssemblyName>
            <SignAssembly>false</SignAssembly>
            <AssemblyOriginatorKeyFile>
            </AssemblyOriginatorKeyFile>
          </PropertyGroup>
          <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
            <DebugSymbols>true</DebugSymbols>
            <DebugType>full</DebugType>
            <Optimize>false</Optimize>
            <OutputPath>bin\Debug\</OutputPath>
            <DefineConstants>TRACE;DEBUG</DefineConstants>
            <ErrorReport>prompt</ErrorReport>
            <WarningLevel>4</WarningLevel>
            <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
          </PropertyGroup>
          <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
            <DebugType>pdbonly</DebugType>
            <Optimize>true</Optimize>
            <OutputPath>bin\Release\</OutputPath>
            <DefineConstants>TRACE</DefineConstants>
            <ErrorReport>prompt</ErrorReport>
            <WarningLevel>4</WarningLevel>
            <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
          </PropertyGroup>
          <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'CramDebug|AnyCPU' ">
            <DebugSymbols>true</DebugSymbols>
            <OutputPath>..\_builds\bin\CramDebug\</OutputPath>
            <DefineConstants>TRACE;DEBUG</DefineConstants>
            <DebugType>full</DebugType>
            <PlatformTarget>AnyCPU</PlatformTarget>
            <ErrorReport>prompt</ErrorReport>
            <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
          </PropertyGroup>
          <ItemGroup>
            <Reference Include="Castle.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=407dd0808d44fbdc, processorArchitecture=MSIL">
              <SpecificVersion>False</SpecificVersion>
              <HintPath>..\Dependencies\Castle.Core.dll</HintPath>
            </Reference>
            <Reference Include="Castle.MicroKernel, Version=1.0.3.0, Culture=neutral, PublicKeyToken=407dd0808d44fbdc, processorArchitecture=MSIL">
              <SpecificVersion>False</SpecificVersion>
              <HintPath>..\Dependencies\Castle.MicroKernel.dll</HintPath>
            </Reference>
            <Reference Include="Castle.Windsor, Version=1.0.3.0, Culture=neutral, PublicKeyToken=407dd0808d44fbdc, processorArchitecture=MSIL">
              <SpecificVersion>False</SpecificVersion>
              <HintPath>..\Dependencies\Castle.Windsor.dll</HintPath>
            </Reference>
            <Reference Include="System" />
            <Reference Include="System.Data" />
            <Reference Include="System.Xml" />
          </ItemGroup>
          <ItemGroup>
            <Compile Include="Bootstrapper.cs" />
            <Compile Include="IStartupModule.cs" />
            <Compile Include="ParameterBuilder.cs" />
            <Compile Include="ServiceLocator.cs" />
            <Compile Include="Properties\AssemblyInfo.cs" />
            <Compile Include="WindsorContainerStartupModule.cs" />
            <Compile Include="With.cs" />
          </ItemGroup>
          <Import Project="$(MSBuildBinPath)\Microsoft.CSharp.targets" />
        </Project>!   
    end

    def test_configuration()
        projectFile = TestableProjectFile.new(ProjectFileTests.ProjectSource)
        assert_equal("Debug", projectFile.configuration)
    end

    def test_project_name()
        projectFile = TestableProjectFile.new(ProjectFileTests.ProjectSource)
        assert_equal("Example.Startup", projectFile.project_name)
    end

    def test_assembly_name()
        projectFile = TestableProjectFile.new(ProjectFileTests.ProjectSource)
        assert_equal("Example.Startup.dll", projectFile.assembly)
    end

    def test_output_path()
        projectFile = TestableProjectFile.new(ProjectFileTests.ProjectSource)
        assert_equal("bin/Debug/", projectFile.output_path)
    end

    def test_assembly_path()
        projectFile = TestableProjectFile.new(ProjectFileTests.ProjectSource)
        assert_equal("bin/Debug/Example.Startup.dll", projectFile.assembly_path)
    end

end
