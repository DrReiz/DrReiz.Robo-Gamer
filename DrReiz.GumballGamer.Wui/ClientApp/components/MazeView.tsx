import * as React from 'react';
import { RouteComponentProps } from 'react-router';

interface MazeState {
    is_grid_display: boolean;
    selectedVisionShot: VisionShot;
    visionShots: VisionShot[];
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
                name: "171213.233229",
                filename: "171213.233229.png",
            },
            visionShots:[],
            };

        this.load();
    }

    async load() {
        let response = await fetch('visionShots');
        let visionShotFilenames = (await response.json()) as string[];
        let visionShots = visionShotFilenames.map(filename => ({ name: filename.replace(".png", ""), filename: filename }));

        this.setState({ visionShots: visionShots });
    }

    public render() {
        //let visionShotFilenames = ["171213.233408.png", "171213.233335.png", "171213.233229.png"];
        //let visionShots = visionShotFilenames.map(filename => ({ name: filename.replace(".png", ""), filename: filename }));
        let visionShots = this.state.visionShots;

        let maze = MazeView.Maze;

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
                    </div>
                    <div style={{ display: 'table-row' }}>
                        <div style={{ width: frame.width, height: frame.height, border: '0px solid black', display: 'table-cell', position: 'relative', color: 'lightblue' }}>
                            <img style={{ width: frame.width, height: frame.height, position: 'absolute' }} src={'data/' + this.state.selectedVisionShot.filename} />
                            {
                              this.state.is_grid_display ? 
                              grid.map((area,k) =>
                                <div style={{ left: area.x, top: area.y, width: area.width, height: area.height, position: 'absolute', border:'1px dashed lightblue' }} key={k}>
                                </div>
                               )
                               :null
                            }
                            {areas.map((area, k) =>
                                <div className="maze-cell" style={{ left: area.x, top: area.y, width: area.width, height: area.height, position: 'absolute', border: (MazeView.IsFloat(area.name) ? '2px solid red' : '0.5px solid lightpink'), zIndex: MazeView.IsFloat(area.name)? 10 : 0 }} key={k}>
                                    <div className="maze-text">{area.name}</div>
                                </div>
                               )
                            }
                        </div>
                        <div style={{ width: '20px', display: 'table-cell' }}></div>
                        <div style={{ width: frame.width, height: frame.height, border: '1px solid black', display: 'table-cell', position:'relative', color:'green' }}>
                            {areas.map((area, k) =>
                                <div className="maze-cell" style={{ left: area.x, top: area.y, width: area.width, height: area.height, position: 'absolute', border: (MazeView.IsFloat(area.name) ? '2px solid black' : '0.5px solid darkgray'), zIndex: MazeView.IsFloat(area.name) ? 10 : 0 }} key={k}>
                                    <div className="maze-text">{area.value}</div>
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
    static Maze: Maze = {
        frame: {
            x: 0, y: 0, width: 900, height: 1600
        },
        perceptionShots:
            [
                {
                    shotName: '171213.233408',
                    areas: [
                        { x: 30, y: 280 + 20, width: 170, height: 156, name: 'cell-0-0', value: 'pit' },
                        { x: 200, y: 280 + 20, width: 170, height: 156, name: 'cell-1-0', value: 'den' },
                        { x: 370, y: 280 + 20, width: 170, height: 156, name: 'cell-2-0', value: 'den' },
                        { x: 540, y: 280 - 30, width: 170, height: 156, name: 'float-3-0', value: 'enemy' },
                        { x: 710, y: 280, width: 170, height: 156, name: 'cell-4-0', value: 'block' },

                        { x: 30, y: 436 + 20, width: 170, height: 156, name: 'cell-0-1', value: 'empty' },
                        { x: 200, y: 436 + 20, width: 170, height: 156, name: 'cell-1-1', value: 'empty' },
                        { x: 370, y: 436 + 20, width: 170, height: 156, name: 'cell-2-1', value: 'empty' },
                        { x: 540, y: 436 + 20, width: 170, height: 156, name: 'cell-3-1', value: 'bee-tree' },
                        { x: 710, y: 436, width: 170, height: 156, name: 'cell-4-1', value: 'block' },

                        { x: 30, y: 592 + 20, width: 170, height: 156, name: 'cell-0-2', value: 'tree' },
                        { x: 200, y: 592 + 20, width: 170, height: 156, name: 'cell-1-2', value: 'empty' },
                        { x: 370, y: 592 + 20, width: 170, height: 156, name: 'cell-2-2', value: 'empty' },
                        { x: 540, y: 592 - 30, width: 170, height: 156, name: 'float-3-2', value: 'key' },
                        { x: 710, y: 592 - 30, width: 170, height: 156, name: 'float-4-2', value: 'enemy' },

                        { x: 30, y: 748 + 20, width: 170, height: 156, name: 'cell-0-3', value: 'door' },
                        { x: 200, y: 748 + 20, width: 170, height: 156, name: 'cell-1-3', value: 'pit' },
                        { x: 370, y: 748 + 20, width: 170, height: 156, name: 'cell-2-3', value: 'empty' },
                        { x: 540, y: 748 + 20, width: 170, height: 156, name: 'cell-3-3', value: 'empty' },
                        { x: 710, y: 748, width: 170, height: 156, name: 'cell-3-3', value: 'block' },

                        { x: 30, y: 904 + 20, width: 170, height: 156, name: 'cell-0-4', value: 'empty' },
                        { x: 200, y: 904 + 20, width: 170, height: 156, name: 'cell-1-4', value: 'empty' },
                        { x: 370, y: 904 + 20, width: 170, height: 156, name: 'cell-2-4', value: 'empty' },
                        { x: 540, y: 904 - 30, width: 170, height: 156, name: 'float-3-4', value: 'EP' },
                        { x: 710, y: 904, width: 170, height: 156, name: 'cell-4-4', value: 'slab' },

                        { x: 30, y: 1060 + 20, width: 170, height: 156 - 20, name: 'cell-0-4', value: 'empty' },
                        { x: 200, y: 1060 - 30, width: 170, height: 156, name: 'float-1-5', value: 'arrow' },
                        { x: 370, y: 1060 - 30, width: 170, height: 156, name: 'float-2-5', value: 'mana' },
                        { x: 540, y: 1060 + 20, width: 170, height: 156 - 20, name: 'cell-3-5', value: 'empty' },
                        { x: 710, y: 1060, width: 170, height: 156, name: 'cell-4-5', value: 'slab' },
                    ]
                },
                {
                    shotName: '171213.233335',
                    areas: [
                        { x: 30, y: 280, width: 170, height: 156, name: 'cell-0-0', value: 'dark-slab' },
                        { x: 200, y: 280, width: 170, height: 156, name: 'cell-1-0', value: 'dark-slab' },
                        { x: 370, y: 280, width: 170, height: 156, name: 'cell-2-0', value: 'dark-slab' },
                        { x: 540, y: 280, width: 170, height: 156, name: 'cell-3-0', value: 'dark-slab' },
                        { x: 710, y: 280, width: 170, height: 156, name: 'cell-4-0', value: 'dark-slab' },

                        { x: 30, y: 436, width: 170, height: 156, name: 'cell-0-1', value: 'light-slab' },
                        { x: 200, y: 436, width: 170, height: 156, name: 'cell-1-1', value: 'dark-slab' },
                        { x: 370, y: 436, width: 170, height: 156, name: 'cell-2-1', value: 'dark-slab' },
                        { x: 540, y: 436, width: 170, height: 156, name: 'cell-3-1', value: 'dark-slab' },
                        { x: 710, y: 436, width: 170, height: 156, name: 'cell-4-1', value: 'dark-slab' },

                        { x: 30, y: 592 + 20, width: 170, height: 156, name: 'cell-0-2', value: 'tree' },
                        { x: 200, y: 592, width: 170, height: 156, name: 'cell-1-2', value: 'light-slab' },
                        { x: 370, y: 592, width: 170, height: 156, name: 'cell-2-2', value: 'dark-slab' },
                        { x: 540, y: 592, width: 170, height: 156, name: 'cell-3-2', value: 'dark-slab' },
                        { x: 710, y: 592, width: 170, height: 156, name: 'cell-4-2', value: 'dark-slab' },

                        { x: 30, y: 748 + 20, width: 170, height: 156, name: 'cell-0-3', value: 'door' },
                        { x: 200, y: 748, width: 170, height: 156, name: 'cell-1-3', value: 'light-slab' },
                        { x: 370, y: 748, width: 170, height: 156, name: 'cell-2-3', value: 'dark-slab' },
                        { x: 540, y: 748, width: 170, height: 156, name: 'cell-3-3', value: 'dark-slab' },
                        { x: 710, y: 748, width: 170, height: 156, name: 'cell-3-3', value: 'dark-slab' },

                        { x: 30, y: 904, width: 170, height: 156, name: 'cell-0-4', value: 'light-slab' },
                        { x: 200, y: 904, width: 170, height: 156, name: 'cell-1-4', value: 'dark-slab' },
                        { x: 370, y: 904, width: 170, height: 156, name: 'cell-2-4', value: 'dark-slab' },
                        { x: 540, y: 904, width: 170, height: 156, name: 'cell-3-4', value: 'dark-slab' },
                        { x: 710, y: 904, width: 170, height: 156, name: 'cell-4-4', value: 'dark-slab' },

                        { x: 30, y: 1060, width: 170, height: 156, name: 'cell-0-4', value: 'dark-slab' },
                        { x: 200, y: 1060 + 20, width: 170, height: 156 - 20, name: 'cell-1-5', value: 'arrow' },
                        { x: 370, y: 1060, width: 170, height: 156, name: 'cell-2-5', value: 'dark-slab' },
                        { x: 540, y: 1060, width: 170, height: 156, name: 'cell-3-5', value: 'dark-slab' },
                        { x: 710, y: 1060, width: 170, height: 156, name: 'cell-4-5', value: 'dark-slab' },
                    ]
                },
                {
                    shotName: '171213.233229',
                    areas: [
                        { x: 30, y: 540, width: 900 - 60, height: 1040 - 540, name: 'popup', value: 'disconnect' },

                    ]
                },
            ]
    };
}

interface Rect {
    x: number;
    y: number;
    width: number;
    height: number;
}

interface Maze {
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
