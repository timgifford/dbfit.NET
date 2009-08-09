require 'fileutils'
require 'rexml/document'
include FileUtils
include REXML

class ProjectFile
    
    attr_accessor :projectXml, :projectFile

    def initialize(project_file)
        self.projectFile = project_file
        self.projectXml = Document.new(File.new(project_file))
    end

    def configuration
        projectXml.elements["Project/PropertyGroup/Configuration"].text
    end

    def project_name
        projectXml.elements["Project/PropertyGroup/AssemblyName"].text
    end

    def assembly
        "#{project_name}.dll"
    end

    def output_path
        path, config = "", configuration
        projectXml.root.each_element("//Project/PropertyGroup") do | element |
            condition = element.attributes["Condition"] if element.attributes["Condition"] != nil
            if(condition != nil && condition.index(config) > 0)
                path = element.elements["OutputPath"].text.gsub("\\", "/")
                break
            end
        end 
        return File.exists?(projectFile) ? "#{projectFile[0, projectFile.rindex("/")]}/#{path}" : path
    end

    def assembly_path
        "#{output_path}#{assembly}"
    end

    def config
        exists = File.exists("#{assembly_path}.config")
        "#{assembly_path}.config" if exists
    end

    def to_s
        puts "Configuration: #{configuration} \n Assembly: #{assembly}"
    end

end
