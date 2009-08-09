
class NUnitResultsEvaluator

    attr_accessor :passed, :failed, :ignored, :elapsed_time

    def initialize()
        self.passed = self.failed = self.ignored = self.elapsed_time = 0
    end

    def print_test_results(output, project_name)
        local_passed = local_failed = local_elapsed_time = 0
        results_found = false
        output.each do | line |
            if md = /Tests run: (\d+?), Errors: (\d+?), Failures: (\d+?), Inconclusive: (\d+?), Time: (\d+.\d+) seconds/.match(line)
                    local_passed       = md[1].to_i
                    local_failed       = md[3].to_i
                    local_elapsed_time = md[5].to_f
                    @passed            += md[1].to_i
                    @failed            += md[3].to_i
                    @elapsed_time      += md[5].to_f
            end
            if md2 = /Not run: (\d+?), Invalid: (\d+?), Ignored: (\d+?), Skipped: (\d+?)/.match(line) 
                results_found = true
                local_ignored = md2[3].to_i
                @ignored += md2[3].to_i
                outputLine = "  Tests run: #{local_passed}, Failures: #{local_failed}, Ignored: #{local_ignored}, Time: #{local_elapsed_time} seconds"
                highlight(outputLine)
            end
        end
        puts "  No tests results for #{project_name}" if !results_found
    end

    def print_statistics()
        puts "\n\e[36;1mTest results:\e[0m"
        puts "\e[36;1mTests run: #{passed}, Failures: #{failed}, Ignored: #{ignored}, Time: #{elapsed_time} seconds\e[0m"
        puts (@failed > 0) ? "\n\e[31;1mTESTS FAILED\e[0m" : "\n\e[32;1mSUCCESS\e[0m"
    end

    def highlight(line)
        highlighted_line = [FAILURE_HIGHLIGHTER, IGNORED_HIGHLIGHTER, PASSED_HIGHLIGHTER, NO_TESTS_HIGHLIGHTER].inject(line) do |line, highlighter|
            highlighter.call(line)
        end
        puts highlighted_line
    end

    FAILURE_HIGHLIGHTER  =  lambda { | line | line.gsub(/(Failures: ([^0]\d+|[^0])),/, "\e[31;1m\\1\e[0m,") }
    IGNORED_HIGHLIGHTER  =  lambda { | line | line.gsub(/(Ignored: ([^0]\d+|[^0])),/, "\e[33;1m\\1\e[0m,") }
    PASSED_HIGHLIGHTER   =  lambda { | line | line.gsub(/(Tests run: ([^0]\d+|[^0])),/, "\e[32;1m\\1\e[0m,")}
    NO_TESTS_HIGHLIGHTER =  lambda { | line | line.gsub(/(Tests run: 0),/, "\e[33;1m\\1\e[0m,")}

end

