import * as React from 'react';
import { RouteComponentProps } from 'react-router';

interface MazeState {
    is_grid_display: boolean;
    selectedVisionShot: VisionShot;
    visionShots: VisionShot[];
    perception: Perception;
}
interface VisionShot {
    name: string;
    filename: string;
}


export class MazeView extends React.Component<RouteComponentProps<{}>, MazeState> {
    constructor() {
        super();
        this.state =
            {
                is_grid_display: false,
                //img_name: "171213.233335.png",
                //img_name: "171213.233408.png",
                selectedVisionShot: {
                    name: "171213.233408",
                    filename: "171213.233408.png",
                },
                visionShots: [],
                perception: {
                    frame: {
                        x: 0, y: 0, width: 900, height: 1600
                    },
                    perceptionShots: []
                }
            };

        this.loadSetting();
        this.load();
        this.loadPerception();

    }

    async loadSetting() {
        let response = await fetch('data/setting.json');
        let setting = (await response.json());

        this.setState({ selectedVisionShot: { name: setting.selectedVisionShot, filename: setting.selectedVisionShot + ".png"} });
    }
    async load() {
        let response = await fetch('visionShots');
        let visionShotFilenames = (await response.json()) as string[];
        let visionShots = visionShotFilenames.map(filename => ({ name: filename.replace(".png", ""), filename: filename }));

        this.setState({ visionShots: visionShots });
    }
    async loadPerception() {
        let response = await fetch('data/perception.json');
        let perception = (await response.json()) as Perception;
        this.setState({ perception:perception });
    }

    public render() {
        //let visionShotFilenames = ["171213.233408.png", "171213.233335.png", "171213.233229.png"];
        //let visionShots = visionShotFilenames.map(filename => ({ name: filename.replace(".png", ""), filename: filename }));
        let visionShots = this.state.visionShots;

        let maze = this.state.perception;

        let visionFrame = maze.perceptionShots.find(shot => shot.shotName == this.state.selectedVisionShot.name);

        let resizeK = 0.4;
        let frame = MazeView.ResizeFrame(maze.frame, resizeK);
        let areas = MazeView.ResizeAreas(visionFrame == null ? [] :  visionFrame.areas, resizeK);
        let grid = MazeView.ResizeRects(MazeView.Grid(maze.frame.width, maze.frame.height, 200), resizeK);
        //console.log(maze);

        return <div>
            <h1>Maze</h1>
            <div className="row">
                <div className="col-sm-2">
                    {
                        visionShots.map((shot, k) =>
                            <div key={k} style={{ color: shot.name == this.state.selectedVisionShot.name ? "blue" : "inherit", cursor: "pointer" }} onClick={() => {this.selectVisionShot(shot)}}>
                                {shot.name}
                            </div>
                        )
                    }
                </div>
                <div className="col-sm-10">
                    <div onClick={() => { this.toggleGridDisplay() }}>grid</div>
                    <div style={{ display: 'table-row' }}>
                        <div style={{ width: frame.width, display: 'table-cell', textAlign: 'center', fontSize: '150%' }}>AI-Gamer Vision
                        </div>
                        <div style={{ width: '20px', display: 'table-cell' }}></div>
                        <div style={{ width: frame.width, display: 'table-cell', textAlign: 'center', fontSize: '150%'}}>AI-Gamer Perception</div>
                        <div style={{ width: '20px', display: 'table-cell' }}></div>
                        <div style={{ width: frame.width, display: 'table-cell', textAlign: 'center', fontSize: '150%' }}>AI-Gamer Mind</div>
                    </div>
                    <div style={{ display: 'table-row' }}>
                        <div style={{ width: frame.width, height: frame.height, border: '0px solid black', display: 'table-cell', position: 'relative', color: 'lightblue' }}>
                            <img style={{ width: frame.width, height: frame.height, position: 'absolute' }} src={'screenshot/' + this.state.selectedVisionShot.name} />
                            {
                              this.state.is_grid_display ? 
                              grid.map((area,k) =>
                                <div style={{ left: area.x, top: area.y, width: area.width, height: area.height, position: 'absolute', border:'1px dashed lightblue' }} key={k}>
                                </div>
                               )
                               :null
                            }
                            {areas.map((area, k) =>
                                <div className="maze-cell" style={{ left: area.x, top: area.y, width: area.width, maxWidth: area.width, height: area.height, position: 'absolute', border: (MazeView.IsFloat(area.name) ? '2px solid red' : '0.5px solid lightpink'), zIndex: MazeView.IsFloat(area.name) ? 10 : 0 }} key={k}>
                                    <div className="maze-text" style={{ left: area.x, top: area.y, width: area.width, maxWidth: area.width, height: area.height }} title={area.value} > { area.name }</div>
                                </div>
                               )
                            }
                        </div>
                        <div style={{ width: '20px', display: 'table-cell' }}></div>
                        <div style={{ width: frame.width, height: frame.height, border: '1px solid black', display: 'table-cell', position:'relative', color:'green' }}>
                            {areas.map((area, k) =>
                                <div className="maze-cell" style={{ left: area.x, top: area.y, width: area.width, maxWidth: area.width, height: area.height, position: 'absolute', border: (MazeView.IsFloat(area.name) ? '2px solid black' : '0.5px solid darkgray'), zIndex: MazeView.IsFloat(area.name) ? 10 : 0 }} key={k}>
                                    <div className="maze-text" style={{ left: area.x, top: area.y, width: area.width, maxWidth: area.width, height: area.height }} title={area.value}>{area.value}</div>
                                </div>
                               )
                            }
                        </div>
                    </div>
                </div>
            </div>

        </div>;
    }
    static IsFloat(name: string | undefined): boolean {
        return name != null && !name.startsWith('cell-');
    }
    static Grid(width: number, height: number, size: number): Rect[] {
        return [...Array(Math.floor((height - 1) / size - 1)).keys()].map(j => { return { x: 0, y: (j + 1) * size, width: width, height: size }; })
            .concat(
            [...Array(Math.floor((width - 1) / size - 1)).keys()].map(i => { return { x: (i + 1) * size, y: 0, width: size, height: height }; })
                );
    }
    static ResizeAreas(areas: Area[], k: number): Area[] {
        return areas.map(area => {
            return {
                x: k * area.x,
                y: k * area.y,
                width: k * area.width,
                height: k * area.height,
                name: area.name,
                value: area.value
            };
        });
    }
    static ResizeFrame(rect: Rect, k:number): Rect {
        return {
            x: k * rect.x,
            y: k * rect.y,
            width: k * rect.width,
            height: k * rect.height,
        };
    }
    static ResizeRects(rects: Rect[], k:number): Rect[] {
        return rects.map(rect => {
            return {
                x: k * rect.x,
                y: k * rect.y,
                width: k * rect.width,
                height: k * rect.height,
            };
        })
    }
    toggleGridDisplay() {
        this.setState({
            is_grid_display: !this.state.is_grid_display
        });
    }
    selectVisionShot(shot: VisionShot) {
        this.setState({
            selectedVisionShot:shot
        });
    }


    //incrementCounter() {
    //    this.setState({
    //        currentCount: this.state.currentCount + 1
    //    });
    //}
}

interface Rect {
    x: number;
    y: number;
    width: number;
    height: number;
}

interface Perception {
    frame: Rect;
    perceptionShots: PerceptionShot[];
}
interface PerceptionShot {
    shotName: string;
    areas: Area[];
}
interface Area extends Rect {
    name?: string;
    value?: string;
}
